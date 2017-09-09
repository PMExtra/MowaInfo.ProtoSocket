namespace MowaInfo.ProtoSocket.Abstract
{
    public interface IMessageContainer
    {
        ulong Id { get; set; }

        ulong? ReplyId { get; set; }

        int MessageType { get; }
    }
}
