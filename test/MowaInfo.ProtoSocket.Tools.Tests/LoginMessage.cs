using MowaInfo.ProtoSocket.Abstract;
using ProtoBuf;

namespace MowaInfo.ProtoSocket.Tools.Tests
{
    [ProtoContract]
    public class LoginMessage : IMessage
    {
        [ProtoMember(1, IsRequired = true)]
        public string UserName { get; set; }

        [ProtoMember(2, IsRequired = true)]
        public string Password { get; set; }
    }
}
