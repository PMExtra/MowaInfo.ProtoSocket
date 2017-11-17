using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using MowaInfo.ProtoSocket.Abstract;
using MowaInfo.ProtoSocket.Commands;

namespace MowaInfo.ProtoSocket.Routing
{
    internal class CommandResolver
    {
        private readonly IReadOnlyDictionary<int, CommandInfo[]> _commandsByMessageType;

        public CommandResolver(IEnumerable<Type> commandTypes)
        {
            var commands = new Dictionary<int, List<CommandInfo>>();

            foreach (var type in commandTypes.OrderBy(type => type.GetOrder()))
            {
#if NETSTANDARD1_3
                var iCommands = type.GetInterfaces().Where(i => i.GetTypeInfo().IsGenericType && i.GetGenericTypeDefinition() == typeof(ICommand<>));
#else
                var iCommands = type.GetInterfaces().Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ICommand<>));
#endif
                foreach (var iCommand in iCommands)
                {
                    var messageClass = iCommand.GenericTypeArguments.Single();
                    var messageType = GetMessageTypeOfClass(messageClass);
                    commands.TryGetValue(messageType, out var list);
                    if (list == null)
                    {
                        list = new List<CommandInfo>();
                        commands[messageType] = list;
                    }
                    list.Add(new CommandInfo
                    {
                        CommandClass = type,
#if NETSTANDARD1_3
                        IsSynchronized = type.GetTypeInfo().GetCustomAttribute<SynchronizeAttribute>()?.Synchronized ?? false,
                        Filters = type.GetTypeInfo().GetCustomAttributes<CommandFilterAttribute>()
                            .OrderBy(filter => filter.Order)
                            .ToArray(),
                        ExceptionHandlers = type.GetTypeInfo().GetCustomAttributes<ExceptionHandlerAttribute>()
                            .OrderBy(filter => filter.Order)
                            .ToArray(),
#else
                        IsSynchronized = type.GetCustomAttribute<SynchronizeAttribute>()?.Synchronized ?? false,
                        Filters = type.GetCustomAttributes<CommandFilterAttribute>()
                            .OrderBy(filter => filter.Order)
                            .ToArray(),
                        ExceptionHandlers = type.GetCustomAttributes<ExceptionHandlerAttribute>()
                            .OrderBy(filter => filter.Order)
                            .ToArray(),
#endif
                        Invoker = type.GetMethod(nameof(ICommand<IMessage>.ExecuteAsync))
                    });
                }
            }

            _commandsByMessageType = commands.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.ToArray());
        }

        public IEnumerable<CommandInfo> CommandsOfMessageType(int messageType)
        {
            return _commandsByMessageType[messageType];
        }

        [SuppressMessage("ReSharper", "SuggestBaseTypeForParameter")]
        private static int GetMessageTypeOfClass(Type messageClass)
        {
#if NETSTANDARD1_3
            var attribute = messageClass.GetTypeInfo().GetCustomAttribute<MessageTypeAttribute>();
#else
            var attribute = messageClass.GetCustomAttribute<MessageTypeAttribute>();
#endif
            if (attribute == null)
            {
                throw new NotImplementedException($"The MessageType of type '{messageClass.Name}' has not been defined.");
            }
            return attribute.MessageType;
        }
    }
}
