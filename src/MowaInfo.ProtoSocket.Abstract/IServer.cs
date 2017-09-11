using System.Threading.Tasks;

namespace MowaInfo.ProtoSocket.Abstract
{
    public interface IServer
    {
        Task StartAsync();

        Task StopAsync();
    }
}
