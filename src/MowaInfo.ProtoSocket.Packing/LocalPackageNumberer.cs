using System.Threading;
using MowaInfo.ProtoSocket.Abstract;

namespace MowaInfo.ProtoSocket.Packing
{
    public class LocalPackageNumberer : IPackageNumberer
    {
        private long _lastId;

        public ulong NextId()
        {
            return (ulong)Interlocked.Increment(ref _lastId);
        }
    }
}
