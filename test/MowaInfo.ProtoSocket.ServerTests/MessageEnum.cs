using ProtoBuf;

namespace MowaInfo.ProtoSocket.ServerTests
{
    [ProtoContract]
    public enum MessageEnum
    {
        [ProtoEnum(Name = nameof(MessageEnum) + nameof(Success))]
        Success = 1
    }
}
