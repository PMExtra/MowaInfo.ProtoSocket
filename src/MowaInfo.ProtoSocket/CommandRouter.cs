using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using DotNetty.Transport.Channels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MowaInfo.ProtoSocket.Abstract;

#if NETSTANDARD1_3
using System.Reflection;
#endif

namespace MowaInfo.ProtoSocket
{
    public class CommandRouter<TContainer> : SimpleChannelInboundHandler<TContainer>
        where TContainer : class, IMessageContainer, new()
    {
        // ReSharper disable once StaticMemberInGenericType
        private static Dictionary<Type, List<Type>> _commands;

        public CommandRouter(IServiceCollection services)
        {
            if (_commands != null) return;
            _commands = new Dictionary<Type, List<Type>>();
            var types = services
                .Select(descriptor => descriptor.ServiceType);

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
                    _commands.TryGetValue(messageType, out var list);
                    if (list == null)
                    {
                        list = new List<Type>();
                        _commands[messageType] = list;
                    }
                    list.Add(type);
                }
            }
        }

        protected override void ChannelRead0(IChannelHandlerContext context, TContainer message)
        {
            var commandSetupHandler = context.Channel.Pipeline.Get<CommandSetupHandler>();

            var commands = _commands[message.GetType()];
            if (commands == null)
            {
                throw new NoMatchedCommandException(typeName);
            }

            foreach (var commandType in commands)
            {
                var command = commandSetupHandler.Provider.GetService(commandType);
                if (command == null)
                {
                    throw new NotImplementedException();
                }

            }
            var attributes = commands.GetTypeInfo().GetCustomAttributes<CommandFilterAttribute>().OrderBy(attribute => attribute.Order);
            var commandContext = new CommandExecutingContext(context, message, command);
            foreach (var attribute in attributes)
            {
                attribute.OnCommandExecuting(commandContext);
                if (commandContext.Cancel) return;
            }
            command.ExecuteCommand(context, message);
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
