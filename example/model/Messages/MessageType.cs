using ProtoBuf;

namespace Messages
{
    [ProtoContract]
    public enum MessageType
    {
        [ProtoEnum(Name = nameof(MessageType) + nameof(Login))]
        Login = 101,

        [ProtoEnum(Name = nameof(MessageType) + nameof(Success))]
        Success = 102
    }
}
