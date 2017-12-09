using System.Threading;
using System.Threading.Tasks;
using MowaInfo.ProtoSocket.Abstract;

namespace MowaInfo.ProtoSocket.Tests
{
    public class LocalPackageNumberer : IPackageNumberer
    {
        private long _lastId;

        public ulong NextId()
        {
            return (ulong)Interlocked.Increment(ref _lastId);
        }

        public Task<ulong> NextIdAsync()
        {
            return Task.FromResult(NextId());
        }

        public void Reset(ulong starting = 0)
        {
            _lastId = (long)starting;
        }

        public Task ResetAsync(ulong starting = 0)
        {
            Reset(starting);
            return Task.CompletedTask;
        }
    }
}
