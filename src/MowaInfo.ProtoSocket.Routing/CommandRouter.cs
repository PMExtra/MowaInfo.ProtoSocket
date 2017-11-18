using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using DotNetty.Transport.Channels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MowaInfo.ProtoSocket.Abstract;
using MowaInfo.ProtoSocket.Commands;
using MowaInfo.ProtoSocket.Packing;

namespace MowaInfo.ProtoSocket.Routing
{
    internal class CommandRouter<TContext, TPackage> : SimpleChannelInboundHandler<TPackage>
        where TPackage : IPackage
        where TContext : ICommandContext
    {
        private readonly CommandResolver _commandResolver;
        private readonly IEnumerable<IExceptionHandler> _exceptionHandlers;
        private readonly IEnumerable<ICommandFilter> _filters;
        private readonly ILogger _logger;
        private readonly IServiceProvider _services;

        private readonly List<Task> _tasks = new List<Task>();

        [SuppressMessage("ReSharper", "SuggestBaseTypeForParameter")]
        public CommandRouter(IServiceCollection services, CommandResolver commandResolver, IEnumerable<ICommandFilter> filters, IEnumerable<IExceptionHandler> exceptionHandlers,
            ILogger<CommandRouter<TContext, TPackage>> logger)
        {
            _filters = filters.OrderBy(filter => filter.Order).ToArray();
            _exceptionHandlers = exceptionHandlers.OrderBy(handler => handler.GetType().GetOrder()).ToArray();

            _logger = logger;
            _commandResolver = commandResolver;

            _services = services.BuildServiceProvider(true);
        }

        protected override void ChannelRead0(IChannelHandlerContext context, TPackage package)
        {
            var scope = _services.CreateScope();
            var services = scope.ServiceProvider;
            foreach (var commandInfo in _commandResolver.CommandsOfMessageType(package.MessageType))
            {
                var command = services.GetRequiredService(commandInfo.CommandClass);
                var commandContext = services.GetRequiredService<ICommandContext>();
                commandContext.RequestServices = services;
                commandContext.Request = package;
                var task = Task.Run(async () =>
                    {
                        try
                        {
                            foreach (var filter in _filters)
                            {
                                commandContext.RequestAborted.ThrowIfCancellationRequested();
                                var filterTask = filter.OnCommandExecuting(commandContext);
                                if (filter.Await)
                                {
                                    await filterTask;
                                }
                            }
                            foreach (var filter in commandInfo.Filters)
                            {
                                commandContext.RequestAborted.ThrowIfCancellationRequested();
                                var filterTask = filter.OnCommandExecuting(commandContext);
                                if (filter.Await)
                                {
                                    await filterTask;
                                }
                            }
                            try
                            {
                                commandContext.RequestAborted.ThrowIfCancellationRequested();
                                await (Task)commandInfo.Invoker.Invoke(command, new object[] { commandContext, package.GetMessage() });
                            }
                            catch (Exception exception)
                            {
                                var solved = false;
                                foreach (var filter in commandInfo.ExceptionHandlers)
                                {
                                    if (filter.HandleExceptionAsync(commandContext, exception).Result)
                                    {
                                        solved = true;
                                    }
                                }
                                if (!solved)
                                {
                                    throw;
                                }
                            }
                            foreach (var filter in commandInfo.Filters)
                            {
                                commandContext.RequestAborted.ThrowIfCancellationRequested();
                                var filterTask = filter.OnCommandExecuted(commandContext);
                                if (filter.Await)
                                {
                                    await filterTask;
                                }
                            }
                            foreach (var filter in _filters)
                            {
                                commandContext.RequestAborted.ThrowIfCancellationRequested();
                                var filterTask = filter.OnCommandExecuted(commandContext);
                                if (filter.Await)
                                {
                                    await filterTask;
                                }
                            }
                        }
                        catch (Exception exception)
                        {
                            var solved = false;
                            foreach (var filter in _exceptionHandlers)
                            {
                                if (filter.HandleExceptionAsync(commandContext, exception).Result)
                                {
                                    solved = true;
                                }
                            }
                            if (!solved)
                            {
                                throw;
                            }
                        }
                    }, commandContext.RequestAborted);
                _tasks.Add(task);
                if (commandInfo.Synchronized)
                {
                    task.GetAwaiter().GetResult();
                    scope.Dispose();
                }
                else
                {
                    task.ContinueWith(t =>
                    {
                        if (t.IsFaulted)
                        {
                            ExceptionCaught(context, t.Exception);
                        }
                        scope.Dispose();
                    });
                }
            }
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
