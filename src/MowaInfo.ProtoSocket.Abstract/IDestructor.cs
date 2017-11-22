using System.Threading.Tasks;

namespace MowaInfo.ProtoSocket.Abstract
{
    public interface IDestructor
    {
        Task Deinit();
    }
}
