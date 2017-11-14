using MowaInfo.ProtoSocket.Abstract;
using ProtoBuf;

namespace Messages
{
    [ProtoContract]
    public class Package : IPackage
    {
        [ProtoMember(101)]
        public LoginMessage Login { get; set; }

        [ProtoMember(15)]
        public SuccessMessage Success { get; set; }

        [ProtoMember(3, IsRequired = true)]
        public int MessageType { get; set; }

        [ProtoMember(1, IsRequired = true)]
        public ulong Id { get; set; }

        [ProtoMember(2)]
        public ulong? ReplyId { get; set; }
    }
}
