using MowaInfo.ProtoSocket.Abstract;
using ProtoBuf;

namespace Messages
{
    [ProtoContract]
    [MessageType((int)MessageType.Success)]
    public class SuccessMessage :IMessage
    {
    }
}
