using System.Threading.Tasks;

namespace MowaInfo.ProtoSocket.Abstract
{
    public interface IPackageNumberer
    {
        ulong NextId();

        Task<ulong> NextIdAsync();

        void Reset(ulong starting = 0);

        Task ResetAsync(ulong starting = 0);
    }
}
