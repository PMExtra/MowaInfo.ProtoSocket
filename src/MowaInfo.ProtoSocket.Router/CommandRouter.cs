using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DotNetty.Transport.Channels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MowaInfo.ProtoSocket.Abstract;

#if NETSTANDARD1_3
using System.Reflection;
#endif

namespace MowaInfo.ProtoSocket.Router
{
    public class CommandRouter<TContext, TContainer> : SimpleChannelInboundHandler<TContainer>
        where TContainer : IMessageContainer
        where TContext : ICommandContext, new()
    {
        private readonly IEnumerable<IExceptionHandler> _exceptionHandlers;
        private readonly IEnumerable<ICommandFilter> _filters;

        private readonly CommandResolver _resolver;
        private readonly IServiceScope _scope;
        private readonly IServiceProvider _services;

        private ISession _session;

        public CommandRouter(IServiceScope serviceScope, IEnumerable<ICommandFilter> filters, IEnumerable<IExceptionHandler> exceptionHandlers)
        {
            _filters = filters;
            _exceptionHandlers = exceptionHandlers;

            _scope = serviceScope;
            _services = serviceScope.ServiceProvider;
            _resolver = _services.GetRequiredService<CommandResolver>();
        }

        protected override void ChannelRead0(IChannelHandlerContext context, TContainer container)
        {
            foreach (var commandInfo in _resolver.CommandsOfMessageType(container.MessageType))
            {
                var command = _services.GetRequiredService(commandInfo.CommandClass);
                var commandContext = new TContext
                {
                    Request = container,
                    RequestServices = _services,
                    Session = _session
                };
                var task = Task.Run(async () =>
                {
                    await _session.LoadAsync();
                    foreach (var filter in _filters)
                    {
                        commandContext.RequestAborted.ThrowIfCancellationRequested();
                        await filter.OnCommandExecuting(commandContext);
                    }
                    foreach (var filter in commandInfo.Filters)
                    {
                        commandContext.RequestAborted.ThrowIfCancellationRequested();
                        await filter.OnCommandExecuting(commandContext);
                    }
                    commandContext.RequestAborted.ThrowIfCancellationRequested();
                    await (Task)commandInfo.Invoker.Invoke(command, new[] { commandContext, container.GetMessage() });
                    foreach (var filter in commandInfo.Filters)
                    {
                        commandContext.RequestAborted.ThrowIfCancellationRequested();
                        await filter.OnCommandExecuted(commandContext);
                    }
                    foreach (var filter in _filters)
                    {
                        commandContext.RequestAborted.ThrowIfCancellationRequested();
                        await filter.OnCommandExecuting(commandContext);
                    }
                    _session = commandContext.Session;
                    await _session.CommitAsync();
                });
                task.Wait(commandContext.RequestAborted);
                task.Exception?.Handle(exception =>
                {
                    foreach (var filter in commandInfo.ExceptionHandlers)
                    {
                        if (filter.HandleException(commandContext, exception).Result) return true;
                    }
                    foreach (var filter in _exceptionHandlers)
                    {
                        if (filter.HandleException(commandContext, exception).Result) return true;
                    }
                    return false;
                });
            }
        }

        public override void ChannelReadComplete(IChannelHandlerContext context)
        {
            context.Flush();
        }

        public override void ChannelUnregistered(IChannelHandlerContext context)
        {
            base.ChannelUnregistered(context);
            _scope.Dispose();
        }

        public override void ExceptionCaught(IChannelHandlerContext context, Exception exception)
        {
            var commandSetupHandler = context.Channel.Pipeline.Get<CommandSetupHandler>();

            commandSetupHandler.Logger.LogError(exception.ToString());
            context.CloseAsync();
        }
    }
}
