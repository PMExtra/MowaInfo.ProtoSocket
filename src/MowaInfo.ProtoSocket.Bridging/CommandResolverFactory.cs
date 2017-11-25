using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using MowaInfo.ProtoSocket.Abstract;

namespace MowaInfo.ProtoSocket.Bridging
{
    internal class CommandResolverFactory
    {
        private readonly Dictionary<Type, Dictionary<int, Type[]>> _wtf;

        public CommandResolverFactory(IEnumerable<Type> commandTypes)
        {
            var commandsByMessageType = new Dictionary<int, List<Type>>();
            var commandsByObserver = new Dictionary<Type, List<Type>>();

            foreach (var type in commandTypes)
            {
#if NETSTANDARD1_5
                var iCommands = type.GetInterfaces().Where(i => i.GetTypeInfo().IsGenericType && i.GetGenericTypeDefinition() == typeof(IBridgeCommand<>));
#else
                var iCommands = type.GetInterfaces().Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IBridgeCommand<>));
#endif
                foreach (var iCommand in iCommands)
                {
                    var messageClass = iCommand.GenericTypeArguments.Single();
                    var messageType = GetMessageTypeOfClass(messageClass);
                    commandsByMessageType.TryGetValue(messageType, out var list);
                    if (list == null)
                    {
                        list = new List<Type>();
                        commandsByMessageType[messageType] = list;
                    }
                    list.Add(type);
                }

#if NETSTANDARD1_5
                var observers = type.GetTypeInfo().GetCustomAttributes<FromObserverAttribute>().Select(attribute => attribute.ObserverType);
#else
                var observers = type.GetCustomAttributes<FromObserverAttribute>().Select(attribute => attribute.ObserverType);
#endif
                foreach (var observer in observers)
                {
                    commandsByObserver.TryGetValue(observer, out var list);
                    if (list == null)
                    {
                        list = new List<Type>();
                        commandsByObserver[observer] = list;
                    }
                    list.Add(type);
                }
            }

            _wtf = commandsByObserver.ToDictionary(kvp => kvp.Key, kvp => commandsByMessageType.ToDictionary(kvp2 => kvp2.Key, kvp2 => kvp.Value.Intersect(kvp2.Value).ToArray()));
        }

        internal ICommandResolver CreateResolver(Type observerType)
        {
            if (!IsSubclassOfRawGeneric(typeof(BridgeObserver<>), observerType))
            {
                throw new ArgumentException($"The '{observerType.Name}' is not a subclass of '{typeof(BridgeObserver<>).Name}'", nameof(observerType));
            }
            if (!_wtf.TryGetValue(observerType, out var result))
            {
                result = new Dictionary<int, Type[]>();
            }
            return new CommandResolver(result);
        }

        [SuppressMessage("ReSharper", "SuggestBaseTypeForParameter")]
        private static int GetMessageTypeOfClass(Type messageClass)
        {
#if NETSTANDARD1_5
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

        private static bool IsSubclassOfRawGeneric(Type generic, Type toCheck)
        {
            while (toCheck != null && toCheck != typeof(object))
            {
#if NETSTANDARD1_5
                var cur = toCheck.GetTypeInfo().IsGenericType ? toCheck.GetGenericTypeDefinition() : toCheck;
                if (generic == cur)
                {
                    return true;
                }
                toCheck = toCheck.GetTypeInfo().BaseType;
#else
                var cur = toCheck.IsGenericType ? toCheck.GetGenericTypeDefinition() : toCheck;
                if (generic == cur)
                {
                    return true;
                }
                toCheck = toCheck.BaseType;
#endif
            }
            return false;
        }
    }
}
