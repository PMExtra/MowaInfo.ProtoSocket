using MowaInfo.ProtoSocket.Abstract;
using ProtoBuf;

namespace ATest
{
    public class Package : IPackage
    {
        [ProtoMember(1)]
        public ulong Id { get; set; }

        [ProtoMember(2)]
        public ulong? ReplyId { get; set; }

        [ProtoMember(3)]
        public int MessageType { get; set; }
    }
}
