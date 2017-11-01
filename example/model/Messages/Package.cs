using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using MowaInfo.ProtoSocket.Abstract;
using ProtoBuf;

namespace Messages
{
    [ProtoContract]
    public partial class Package : IPackage
    {
        [ProtoMember(3, IsRequired = true)]
        public MessageType MessageType { get; set; }

        [ProtoMember(1, IsRequired = true)]
        public ulong Id { get; set; }

        [ProtoMember(2)]
        public ulong? ReplyId { get; set; }

        [ProtoMember(4)]
        public LoginMessage Login { get; set; }

        [ProtoMember(5)]
        public SuccessMessage Success { get; set; }
    }

    public partial class Package
    {
        public Package()
        {
        }

        public Package(MessageType type)
        {
            MessageType = type;
        }

        public Package(IMessage message) : this(MessageTypeOfClass(message.GetType()))
        {
            PropertyOfMessageType(MessageType).SetValue(this, message);
        }
    }

    public partial class Package
    {
        int IPackage.MessageType => (int)MessageType;

        Type IPackage.ClassOfMessageType(int messageType)
        {
            return ClassOfMessageType((MessageType)messageType);
        }

        object IPackage.MessageOfMessageType(int messageType)
        {
            return MessageOfMessageType((MessageType)messageType);
        }

        int IPackage.MessageTypeOfClass(Type messageClass)
        {
            return (int)MessageTypeOfClass(messageClass);
        }

        public static Type ClassOfMessageType(MessageType messageType)
        {
            if (!Enum.IsDefined(typeof(MessageType), messageType))
            {
                throw new ArgumentException(nameof(messageType));
            }
            if (!ClassesByMessageType.TryGetValue((int)messageType, out var messageClass))
            {
                throw new ArgumentException($"The package does not have a message of type '{messageType}'.", nameof(messageType));
            }
            return messageClass;
        }

        public object MessageOfMessageType(MessageType messageType)
        {
            if (!Enum.IsDefined(typeof(MessageType), messageType))
            {
                throw new ArgumentException(nameof(messageType));
            }
            return PropertyOfMessageType(messageType).GetValue(this);
        }

        public static MessageType MessageTypeOfClass(Type messageClass)
        {
            if (!MessageTypesByClass.TryGetValue(messageClass, out var messageType))
            {
                throw new NotImplementedException($"The MessageType of type '{messageClass.Name}' has not been defined.");
            }
            return (MessageType)messageType;
        }
    }

    public partial class Package
    {
        private static readonly IReadOnlyDictionary<Type, int> MessageTypesByClass;
        private static readonly IReadOnlyDictionary<int, Type> ClassesByMessageType;
        private static readonly IReadOnlyDictionary<int, PropertyInfo> PropertiesByMessageType;

        static Package()
        {
            var properties = typeof(Package)
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(property => typeof(IMessage).IsAssignableFrom(property.PropertyType))
                .ToArray();

            PropertiesByMessageType = properties
                .ToDictionary(property => GetMessageTypeOfClass(property.PropertyType), property => property);
            MessageTypesByClass = PropertiesByMessageType
                .ToDictionary(kvp => kvp.Value.PropertyType, kvp => kvp.Key);
            ClassesByMessageType = MessageTypesByClass.ToDictionary(kvp => kvp.Value, kvp => kvp.Key);
        }

        private static PropertyInfo PropertyOfMessageType(MessageType messageType)
        {
            if (!PropertiesByMessageType.TryGetValue((int)messageType, out var property))
            {
                throw new ArgumentException($"The package does not have a message of type '{messageType}'.", nameof(messageType));
            }
            return property;
        }

        [SuppressMessage("ReSharper", "SuggestBaseTypeForParameter")]
        private static int GetMessageTypeOfClass(Type messageClass)
        {
            var attribute = messageClass.GetCustomAttribute<MessageTypeAttribute>();
            if (attribute == null)
            {
                throw new NotImplementedException($"The MessageType of type '{messageClass.Name}' has not been defined.");
            }
            return attribute.MessageType;
        }
    }
}
