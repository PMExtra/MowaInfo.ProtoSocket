namespace MowaInfo.ProtoSocket.Abstract
{
    public interface IMessageContainer
    {
        ulong Id { get; }

        ulong? ReplyId { get; }

        int MessageType { get; }
    }
}
