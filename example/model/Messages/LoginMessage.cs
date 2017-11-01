using MowaInfo.ProtoSocket.Abstract;
using ProtoBuf;

namespace Messages
{
    [ProtoContract]
    [MessageType((int)MessageType.Login)]
    public class LoginMessage : IMessage
    {
        [ProtoMember(1, IsRequired = true)]
        public string UserName { get; set; }

        [ProtoMember(2, IsRequired = true)]
        public string Password { get; set; }
    }
}
