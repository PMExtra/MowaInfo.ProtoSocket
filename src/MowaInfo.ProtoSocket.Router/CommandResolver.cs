using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using MowaInfo.ProtoSocket.Abstract;

namespace MowaInfo.ProtoSocket.Router
{
    internal class CommandResolver
    {
        private readonly IReadOnlyDictionary<int, CommandInfo[]> _commandsByCommandType;

        public CommandResolver(IEnumerable<Type> commandTypes)
        {
            var commands = new Dictionary<int, List<CommandInfo>>();

            foreach (var type in commandTypes)
            {
#if NETSTANDARD1_3
                var iCommands = type.GetInterfaces().Where(i => i.GetTypeInfo().IsGenericType && i.GetGenericTypeDefinition() == typeof(ICommand<>)).ToArray();
#else
                var iCommands = type.GetInterfaces().Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ICommand<>)).ToArray();
#endif
                Debug.Assert(!iCommands.Any());
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
                        Filters = type.GetTypeInfo().GetCustomAttributes<CommandFilterAttribute>()
                            .OrderBy(filter => filter.Order)
                            .ToArray(),
                        ExceptionHandlers = type.GetTypeInfo().GetCustomAttributes<ExceptionHandlerAttribute>()
                            .OrderBy(filter => filter.Order)
                            .ToArray(),
#else
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

            _commandsByCommandType = commands.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.ToArray());
        }

        public IEnumerable<CommandInfo> CommandsOfMessageType(int messageType)
        {
            return _commandsByCommandType[messageType];
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
