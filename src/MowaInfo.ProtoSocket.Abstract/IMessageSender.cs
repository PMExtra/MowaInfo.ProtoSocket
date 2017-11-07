using System.Threading.Tasks;

namespace MowaInfo.ProtoSocket.Abstract
{
    public interface IMessageSender
    {
        TaskCompletionSource<IPackage> Send<T>(T message) where T : IMessage;

        TaskCompletionSource<IPackage> Reply<T>(ulong id, T message) where T : IMessage;
    }
}
