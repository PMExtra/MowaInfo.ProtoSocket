namespace MowaInfo.ProtoSocket.Abstract
{
    public interface IChannel
    {
        void Send<T>(T message) where T : IMessage;

        void Reply<T>(ulong id, T message) where T : IMessage;

        void Close();
    }
}
