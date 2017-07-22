using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using MowaInfo.ProtoSocket.Abstract;
using ProtoBuf;

namespace MowaInfo.ProtoSocket.Server.Tests
{
    [ProtoContract]
    public class MessageContainer : IMessageContainer<MessageEnum>
    {
        static MessageContainer()
        {
            var properties = typeof(MessageContainer).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(property => typeof(IMessage<MessageEnum>).IsAssignableFrom(property.PropertyType))
                .ToDictionary(property => property.PropertyType, property => property);
            Properties = new ReadOnlyDictionary<Type, PropertyInfo>(properties);
        }

        public MessageContainer(uint id, uint? replyId)
        {
            Id = id;
            ReplyId = replyId;
            Type = MessageEnum.Success;
        }

        public MessageContainer()
        {
        }

        public static ReadOnlyDictionary<Type, PropertyInfo> Properties { get; }

        [ProtoMember(4)]
        public string MessageContent { get; set; }

        [ProtoMember(1, IsRequired = true)]
        public uint Id { get; set; }

        [ProtoMember(2)]
        public uint? ReplyId { get; set; }

        [ProtoMember(3)]
        public MessageEnum Type { get; set; }
    }
}
