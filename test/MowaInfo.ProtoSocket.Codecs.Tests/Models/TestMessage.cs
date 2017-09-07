using MowaInfo.ProtoSocket.Abstract;
using ProtoBuf;

namespace MowaInfo.ProtoSocket.Codecs.Tests.Models
{
    [ProtoContract]
    public class TestMessage : IMessage
    {
        public MessageType MessageType => MessageType.Test;

        [ProtoMember(1)]
        public string TestContent { get; set; }

        int IMessage.MessageType => (int)MessageType;
    }
}
