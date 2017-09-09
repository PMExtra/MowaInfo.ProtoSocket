using System;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using MowaInfo.ProtoSocket.Abstract;
using ProtoBuf;

namespace MowaInfo.ProtoSocket.Codecs.Tests.Models
{
    [ProtoContract]
    public sealed class MessageContainer : IMessageContainer
    {
        private static readonly ImmutableDictionary<Type, int> MessageTypes;
        private static readonly ImmutableDictionary<int, PropertyInfo> MessageProperties;

        static MessageContainer()
        {
            var properties = typeof(MessageContainer)
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(property => typeof(IMessage).IsAssignableFrom(property.PropertyType))
                .ToArray();

            Debug.Assert(properties.All(property => property.PropertyType.GetCustomAttribute<MessageTypeAttribute>() != null));
            Debug.Assert(properties.GroupBy(property => property.PropertyType).All(group => group.Count() == 1));

            MessageProperties = properties
                .ToImmutableDictionary(property => property.PropertyType.GetCustomAttribute<MessageTypeAttribute>().MessageType, property => property);
            MessageTypes = MessageProperties
                .ToImmutableDictionary(kvp => kvp.Value.PropertyType, kvp => kvp.Key);
        }

        public MessageContainer()
        {
        }

        public MessageContainer(MessageType type)
        {
            MessageType = type;
        }

        public MessageContainer(IMessage message)
        {
            var rawType = MessageTypes[message.GetType()];
            if (!Enum.IsDefined(typeof(MessageType), rawType))
            {
                throw new ArgumentException("The type of message is out of range.", nameof(message));
            }
            MessageType = (MessageType)rawType;
            var property = MessageProperties[rawType];
            if (property == null)
            {
                throw new ArgumentException("The type of message is not defined in container.", nameof(message));
            }
            property.SetValue(this, message);
        }

        [ProtoMember(3, IsRequired = true)]
        public MessageType MessageType { get; set; }

        [ProtoMember(4)]
        public TestMessage Test { get; set; }

        [ProtoMember(1, IsRequired = true)]
        public ulong Id { get; set; }

        [ProtoMember(2)]
        public ulong? ReplyId { get; set; }

        int IMessageContainer.MessageType => (int)MessageType;
    }
}
