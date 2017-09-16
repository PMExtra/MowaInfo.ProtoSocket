using ProtoBuf;

namespace MowaInfo.ProtoSocket.Tools.Tests
{
    [ProtoContract]
    public enum MessageType
    {
        [ProtoEnum(Name = nameof(MessageType) + nameof(Login))]
        Login = 101
    }
}
