using ProtoBuf;

namespace MowaInfo.ProtoSocket.Client.Tests
{
    [ProtoContract]
    public enum MessageEnum
    {
        [ProtoEnum(Name = nameof(MessageEnum) + nameof(Success))]
        Success = 1
    }
}
