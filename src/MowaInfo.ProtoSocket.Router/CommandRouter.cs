using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DotNetty.Common.Utilities;
using DotNetty.Transport.Channels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MowaInfo.ProtoSocket.Abstract;
using MowaInfo.ProtoSocket.Session;

#if NETSTANDARD1_3
using System.Reflection;
#endif

namespace MowaInfo.ProtoSocket.Router
{
    public class CommandRouter<TContext, TContainer> : SimpleChannelInboundHandler<TContainer>
        where TContainer : IPackage
        where TContext : ICommandContext
    {
        private readonly IEnumerable<IExceptionHandler> _exceptionHandlers;
        private readonly IEnumerable<ICommandFilter> _filters;

        private readonly CommandResolver _resolver;
        private readonly IServiceProvider _services;
        private IServiceScope _scope;
        private ILogger _logger;

        private ISession _session;

        public CommandRouter(IServiceProvider services,IEnumerable<ICommandFilter> filters, IEnumerable<IExceptionHandler> exceptionHandlers)
        {
            _filters = filters;
            _exceptionHandlers = exceptionHandlers;

            _services = services;
            _resolver = _services.GetRequiredService<CommandResolver>();
            _logger = _services.GetService<ILoggerFactory>().CreateLogger("CommandRouter");
        }

        public override void ChannelActive(IChannelHandlerContext context)
        {
            _session = context.Channel.GetAttribute(AttributeKey<ISession>.ValueOf(nameof(_session))).Get();
            base.ChannelActive(context);
            _scope = _services.CreateScope();
        }

        protected override void ChannelRead0(IChannelHandlerContext context, TContainer package)
        {
            foreach (var commandInfo in _resolver.CommandsOfMessageType(package.MessageType))
            {
                var command = _services.GetRequiredService(commandInfo.CommandClass);
                var commandContext = _services.GetService<ICommandContext>();
                commandContext.RequestServices = _services;
                commandContext.Session = _session;
                commandContext.Request = package;
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
                    await (Task)commandInfo.Invoker.Invoke(command, new[] { commandContext, package.GetMessage() });
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

        public override void ChannelInactive(IChannelHandlerContext context)
        {
            base.ChannelInactive(context);
            _scope.Dispose();
        }

        public override void ExceptionCaught(IChannelHandlerContext context, Exception exception)
        {
            _logger.LogError(exception.ToString());
            context.CloseAsync();
        }
    }
}
