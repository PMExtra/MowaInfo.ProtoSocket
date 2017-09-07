using ProtoBuf;

namespace MowaInfo.ProtoSocket.Codecs.Tests.Models
{
    [ProtoContract]
    public enum MessageType
    {
        [ProtoEnum(Name = nameof(MessageType) + nameof(Test))]
        Test = 1
    }
}
