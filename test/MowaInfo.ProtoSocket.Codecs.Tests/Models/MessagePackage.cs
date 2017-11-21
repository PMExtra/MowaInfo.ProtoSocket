using System;
using MowaInfo.ProtoSocket.Abstract;
using ProtoBuf;

namespace MowaInfo.ProtoSocket.Codecs.Tests.Models
{
    [ProtoContract]
    public sealed class MessagePackage : IPackage
    {
        [ProtoMember(3, IsRequired = true)]
        public MessageType MessageType { get; set; }

        [ProtoMember(4)]
        public TestMessage Test { get; set; }

        [ProtoMember(1, IsRequired = true)]
        public ulong Id { get; set; }

        [ProtoMember(2)]
        public ulong? ReplyId { get; set; }

        int IPackage.MessageType
        {
            get => (int)MessageType;
            set
            {
                if (Enum.IsDefined(typeof(MessageType), value))
                {
                    MessageType = (MessageType)value;
                }
                else
                {
                    throw new ArgumentOutOfRangeException(nameof(value));
                }
            }
        }
    }
}
