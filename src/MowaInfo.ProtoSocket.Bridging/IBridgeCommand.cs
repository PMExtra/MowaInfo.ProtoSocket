using System.Threading.Tasks;
using MowaInfo.ProtoSocket.Abstract;

namespace MowaInfo.ProtoSocket.Bridging
{
    public interface IBridgeCommand<T>
        where T : IMessage
    {
        Task ExecuteAsync(T message);
    }
}
