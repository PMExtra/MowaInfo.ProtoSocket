using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MowaInfo.ProtoSocket.Abstract;
using ProtoBuf;

namespace MowaInfo.ProtoSocket.Codecs.Tests.Models
{
    [ProtoContract]
    public class MessageContainer : IMessageContainer
    {
        private static readonly Dictionary<Type, PropertyInfo> MessageProperties;

        static MessageContainer()
        {
            MessageProperties = typeof(MessageContainer)
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(property => typeof(IMessage).IsAssignableFrom(property.PropertyType))
                .ToDictionary(property => property.PropertyType, property => property);
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
            if (!Enum.IsDefined(typeof(MessageType), message.MessageType))
            {
                throw new ArgumentException("The type of message is out of range.", nameof(message));
            }
            MessageType = (MessageType)message.MessageType;
            var property = MessageProperties[message.GetType()];
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
