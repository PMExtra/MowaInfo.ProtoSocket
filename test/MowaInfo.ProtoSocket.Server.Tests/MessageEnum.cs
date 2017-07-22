using ProtoBuf;

namespace MowaInfo.ProtoSocket.Server.Tests
{
    [ProtoContract]
    public enum MessageEnum
    {
        [ProtoEnum(Name = nameof(MessageEnum) + nameof(Success))]
        Success = 1
    }
}
