using MowaInfo.ProtoSocket.Abstract;
using ProtoBuf;

namespace MowaInfo.ProtoSocket.Codecs.Tests.Models
{
    [ProtoContract]
    [MessageType((int)MessageType.Test)]
    public class TestMessage : IMessage
    {
        [ProtoMember(1)]
        public string TestContent { get; set; }
    }
}
