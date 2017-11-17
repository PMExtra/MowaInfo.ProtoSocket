using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DotNetty.Transport.Channels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MowaInfo.ProtoSocket.Abstract;
using MowaInfo.ProtoSocket.Commands;
using MowaInfo.ProtoSocket.Packing;
using IChannel = MowaInfo.ProtoSocket.Abstract.IChannel;

namespace MowaInfo.ProtoSocket.Routing
{
    public class CommandRouter<TContext, TPackage> : SimpleChannelInboundHandler<TPackage>
        where TPackage : IPackage
        where TContext : ICommandContext
    {
        private readonly IChannel _channel;
        private readonly IEnumerable<IExceptionHandler> _exceptionHandlers;
        private readonly IEnumerable<ICommandFilter> _filters;
        private readonly ILogger _logger;
        private readonly IServiceProvider _provider;
        private IServiceScope _scope;

        private ISession _session;

        public CommandRouter(IServiceCollection services, ISession session, IChannel channel,
            IEnumerable<ICommandFilter> filters, IEnumerable<IExceptionHandler> exceptionHandlers, ILogger<CommandRouter<TContext, TPackage>> logger)
        {
            _filters = filters.OrderBy(filter => filter.Order).ToArray();
            _exceptionHandlers = exceptionHandlers.OrderBy(handler => handler.GetType().GetOrder()).ToArray();

            _session = session;
            _channel = channel;
            _logger = logger;

            services.AddSingleton(session);
            services.AddSingleton(channel);
            _provider = services.BuildServiceProvider(true);
        }

        protected override void ChannelRead0(IChannelHandlerContext context, TPackage package)
        {
            _scope = _provider.CreateScope();
            var services = _scope.ServiceProvider;
            var resolver = services.GetService<CommandResolver>();
            foreach (var commandInfo in resolver.CommandsOfMessageType(package.MessageType))
            {
                var command = services.GetRequiredService(commandInfo.CommandClass);
                var commandContext = services.GetRequiredService<ICommandContext>();
                commandContext.RequestServices = services;
                commandContext.Session = _session;
                commandContext.Request = package;
                var awaiter = Task.Run(async () =>
                {
                    await _session.LoadAsync();
                    try
                    {
                        foreach (var filter in _filters)
                        {
                            commandContext.RequestAborted.ThrowIfCancellationRequested();
                            var task = filter.OnCommandExecuting(commandContext);
                            if (filter.Await)
                            {
                                await task;
                            }
                        }
                        foreach (var filter in commandInfo.Filters)
                        {
                            commandContext.RequestAborted.ThrowIfCancellationRequested();
                            var task = filter.OnCommandExecuting(commandContext);
                            if (filter.Await)
                            {
                                await task;
                            }
                        }
                        commandContext.RequestAborted.ThrowIfCancellationRequested();
                        await (Task)commandInfo.Invoker.Invoke(command, new object[] { commandContext, package.GetMessage() });
                        foreach (var filter in commandInfo.Filters)
                        {
                            commandContext.RequestAborted.ThrowIfCancellationRequested();
                            var task = filter.OnCommandExecuted(commandContext);
                            if (filter.Await)
                            {
                                await task;
                            }
                        }
                        foreach (var filter in _filters)
                        {
                            commandContext.RequestAborted.ThrowIfCancellationRequested();
                            var task = filter.OnCommandExecuted(commandContext);
                            if (filter.Await)
                            {
                                await task;
                            }
                        }
                    }
                    catch (Exception exception)
                    {
                        var solved = false;
                        foreach (var filter in commandInfo.ExceptionHandlers)
                        {
                            if (filter.HandleExceptionAsync(exception).Result)
                            {
                                solved = true;
                            }
                        }
                        foreach (var filter in _exceptionHandlers)
                        {
                            if (filter.HandleExceptionAsync(exception).Result)
                            {
                                solved = true;
                            }
                        }
                        if (!solved)
                        {
                            throw;
                        }
                    }
                    _session = commandContext.Session;
                    await _session.CommitAsync();
                }, commandContext.RequestAborted).GetAwaiter();
                if (commandInfo.IsSynchronized)
                {
                    awaiter.GetResult();
                }
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
