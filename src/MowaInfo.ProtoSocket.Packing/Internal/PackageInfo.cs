using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using MowaInfo.ProtoSocket.Abstract;

namespace MowaInfo.ProtoSocket.Packing.Internal
{
    [SuppressMessage("ReSharper", "StaticMemberInGenericType")]
    internal static class PackageInfo<T> where T : IPackage
    {
        private static readonly IReadOnlyDictionary<int, Type> ClassesByMessageType;
        private static readonly IReadOnlyDictionary<Type, int> MessageTypesByClass;
        private static readonly IReadOnlyDictionary<int, PropertyInfo> PropertiesByMessageType;

        static PackageInfo()
        {
            var properties = typeof(T)
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(property => typeof(IMessage).IsAssignableFrom(property.PropertyType))
                .ToArray();

            PropertiesByMessageType = properties
                .ToDictionary(property => GetMessageTypeOfClass(property.PropertyType), property => property);

            MessageTypesByClass = PropertiesByMessageType
                .ToDictionary(kvp => kvp.Value.PropertyType, kvp => kvp.Key);

            ClassesByMessageType = MessageTypesByClass
                .ToDictionary(kvp => kvp.Value, kvp => kvp.Key);
        }

        private static PropertyInfo PropertyOfMessageType(int messageType)
        {
            if (!PropertiesByMessageType.TryGetValue(messageType, out var property))
            {
                throw new ArgumentException($"The package does not have a message of type '{messageType}'.", nameof(messageType));
            }
            return property;
        }

        [SuppressMessage("ReSharper", "SuggestBaseTypeForParameter")]
        private static int GetMessageTypeOfClass(Type messageClass)
        {
            var attribute = messageClass.GetTypeInfo().GetCustomAttribute<MessageTypeAttribute>();
            if (attribute == null)
            {
                throw new NotImplementedException($"The MessageType of type '{messageClass.Name}' has not been defined.");
            }
            return attribute.MessageType;
        }

        public static IMessage GetMessage(T package)
        {
            return (IMessage)PropertyOfMessageType(package.MessageType).GetValue(package);
        }

        public static void SetMessage(T package, IMessage value)
        {
            package.MessageType = MessageTypeOfClass(value.GetType());
            PropertyOfMessageType(package.MessageType).SetValue(package, value);
        }

        public static int MessageTypeOfClass(Type messageClass)
        {
            if (!MessageTypesByClass.TryGetValue(messageClass, out var messageType))
            {
                throw new NotImplementedException($"The MessageType of type '{messageClass.Name}' has not been defined.");
            }
            return messageType;
        }

        public static Type ClassOfMessageType(int messageType)
        {
            if (!ClassesByMessageType.TryGetValue(messageType, out var messageClass))
            {
                throw new ArgumentException($"The package does not have a message of type '{messageType}'.", nameof(messageType));
            }
            return messageClass;
        }
    }
}
