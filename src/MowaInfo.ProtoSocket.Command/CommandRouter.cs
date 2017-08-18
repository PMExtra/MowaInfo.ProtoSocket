using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using DotNetty.Transport.Channels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MowaInfo.ProtoSocket.Abstract;

namespace MowaInfo.ProtoSocket.Command
{
    public class CommandRouter<TContainer, TEnum> : SimpleChannelInboundHandler<TContainer>
        where TEnum : struct, IConvertible
        where TContainer : class, IMessageContainer<TEnum>, new()
    {
        // ReSharper disable once StaticMemberInGenericType
        private static Dictionary<string, Type> _dictionary;

        public CommandRouter(IServiceCollection services)
        {
            if (_dictionary != null) return;
            _dictionary = new Dictionary<string, Type>();
            var types = services.Select(descriptor => descriptor.ServiceType).Where(type => typeof(ICommand<IChannelHandlerContext, TContainer>).IsAssignableFrom(type));
            var provider = services.BuildServiceProvider();
            foreach (var type in types)
            {
                if (provider.GetService(type) is ICommand<IChannelHandlerContext, TContainer> command)
                {
                    _dictionary.Add(command.Name, type);
                }
            }
        }

        protected override void ChannelRead0(IChannelHandlerContext context, TContainer message)
        {
            var commandSetupHandler = context.Channel.Pipeline.Get<CommandSetupHandler>();

            var typeName = message.Type.ToString(CultureInfo.InvariantCulture);
            var type = _dictionary[typeName];
            if (type == null)
            {
                throw new NoMatchedCommandException(typeName);
            }

            var command = commandSetupHandler.Provider.GetService(type) as ICommand<IChannelHandlerContext, TContainer>;
            if (command == null)
            {
                throw new NoMatchedCommandException(typeName);
            }
            var attributes = type.GetTypeInfo().GetCustomAttributes<CommandFilterAttribute>().OrderBy(attribute => attribute.Order);
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
