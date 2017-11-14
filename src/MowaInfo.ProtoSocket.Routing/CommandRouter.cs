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

namespace MowaInfo.ProtoSocket.Routing
{
    public class CommandRouter<TContext, TPackage> : SimpleChannelInboundHandler<TPackage>
        where TPackage : IPackage
        where TContext : ICommandContext
    {
        private readonly IEnumerable<IExceptionHandler> _exceptionHandlers;
        private readonly IEnumerable<ICommandFilter> _filters;

        private readonly IMessageSender _sender;
        private readonly ILogger _logger;
        private readonly IServiceProvider _provider;
        private IServiceScope _scope;

        private ISession _session;

        public CommandRouter(IServiceCollection services, ISession session, IMessageSender sender,
            IEnumerable<ICommandFilter> filters, IEnumerable<IExceptionHandler> exceptionHandlers, ILogger<CommandRouter<TContext, TPackage>> logger)
        {
            _filters = filters;
            _exceptionHandlers = exceptionHandlers;

            _session = session;
            _sender = sender;
            _logger = logger;

            services.AddSingleton(session);
            services.AddSingleton(sender);
            _provider = services.BuildServiceProvider(true);
        }

        protected override void ChannelRead0(IChannelHandlerContext context, TPackage package)
        {
            _scope = _provider.CreateScope();
            var services = _scope.ServiceProvider;
            var _resolver = services.GetService<CommandResolver>();
            foreach (var commandInfo in _resolver.CommandsOfMessageType(package.MessageType))
            {
                var command = services.GetRequiredService(commandInfo.CommandClass);
                var commandContext = services.GetService<ICommandContext>();
                commandContext.RequestServices = services;
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
                    await (Task)commandInfo.Invoker.Invoke(command, new object[] { commandContext, package.GetMessage() });
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
            base.ChannelReadComplete(context);
            _scope?.Dispose();
        }

        public override void ExceptionCaught(IChannelHandlerContext context, Exception exception)
        {
            _logger.LogError(exception.ToString());
            Console.WriteLine(exception.GetType().FullName);
            Console.WriteLine(exception.ToString());
            Console.WriteLine(exception.StackTrace);
            context.CloseAsync();
        }
    }
}
