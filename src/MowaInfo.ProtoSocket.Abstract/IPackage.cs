namespace MowaInfo.ProtoSocket.Abstract
{
    public interface IPackage
    {
        ulong Id { get; set; }

        ulong? ReplyId { get; set; }

        int MessageType { get; set; }
    }
}
