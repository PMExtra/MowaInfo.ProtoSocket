using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DotNetty.Transport.Channels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MowaInfo.ProtoSocket.Abstract;

#if NETSTANDARD1_3
using System.Reflection;
#endif

namespace MowaInfo.ProtoSocket.Commands
{
    public class CommandRouter<TContainer> : SimpleChannelInboundHandler<TContainer>
        where TContainer : class, IMessageContainer, new()
    {
        private readonly IReadOnlyDictionary<int, Type[]> _commands;
        private readonly IReadOnlyDictionary<Type, CommandFilterAttribute[]> _filters;
        private readonly IReadOnlyDictionary<Type, MethodInfo> _delegates;

        public CommandRouter(IServiceCollection services)
        {
            var types = services
                .Select(descriptor => descriptor.ServiceType);

            var commands = new Dictionary<int, List<Type>>();
            var filters = new Dictionary<Type, CommandFilterAttribute[]>();
            var delegates = new Dictionary<Type, MethodInfo>();

            foreach (var type in types)
            {
#if NETSTANDARD1_3
                var interfaces = type.GetInterfaces().Where(i => i.GetTypeInfo().IsGenericType && i.GetGenericTypeDefinition() == typeof(ICommand<>));
#else
                var interfaces = type.GetInterfaces().Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ICommand<>));
#endif
                foreach (var @interface in interfaces)
                {
                    var messageType = @interface.GenericTypeArguments.Single();
                    if (!messageType.GetInterfaces().Contains(typeof(IMessage)))
                    {
                        throw new NotImplementedException($"Interface 'IMessage' is not implemented by type '{messageType}'.");
                    }
                    var typeId = messageType.GetCustomAttribute<MessageTypeAttribute>().MessageType;
                    commands.TryGetValue(typeId, out var list);
                    if (list == null)
                    {
                        list = new List<Type>();
                        commands[typeId] = list;
                    }
                    list.Add(type);
                    filters[type] = type.GetCustomAttributes<CommandFilterAttribute>()
                        .OrderBy(filter => filter.Order)
                        .ToArray();
                    delegates[type] = type.GetMethod(nameof(ICommand<IMessage>.ExecuteAsync));
                }
            }

            _commands = commands.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.ToArray());
            _filters = filters;
            _delegates = delegates;
        }

        protected override void ChannelRead0(IChannelHandlerContext context, TContainer continaer)
        {
            var commandSetupHandler = context.Channel.Pipeline.Get<CommandSetupHandler>();

            var commands = _commands[continaer.MessageType];

            foreach (var commandType in commands)
            {
                var command = commandSetupHandler.Provider.GetService(commandType);
                if (command == null)
                {
                    throw new NotImplementedException();
                }
                foreach (var filter in _filters[commandType])
                {
                    filter.OnCommandExecuting();
                }
                var message = container.
                _delegates[commandType].Invoke(command, )
            }
            foreach (var attribute in attributes)
            {
                if (commandContext.Cancel) return;
                attribute.OnCommandExecuted(commandContext);
            }
        }

        public override void ChannelReadComplete(IChannelHandlerContext context)
        {
            context.Flush();
        }

        public override void ExceptionCaught(IChannelHandlerContext context, Exception exception)
        {
            var commandSetupHandler = context.Channel.Pipeline.Get<CommandSetupHandler>();

            commandSetupHandler.Logger.LogError(exception.ToString());
            context.CloseAsync();
        }
    }
}
