using ProtoBuf;

namespace MowaInfo.ProtoSocket.ClientTests
{
    [ProtoContract]
    public enum MessageEnum
    {
        [ProtoEnum(Name = nameof(MessageEnum) + nameof(Success))]
        Success = 1
    }
}
