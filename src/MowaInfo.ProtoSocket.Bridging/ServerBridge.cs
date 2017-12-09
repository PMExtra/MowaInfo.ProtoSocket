using System.Collections.Concurrent;
using System.Threading.Tasks;
using MowaInfo.ProtoSocket.Abstract;
using StackExchange.Redis;

namespace MowaInfo.ProtoSocket.Bridging
{
    public class ServerBridge<TPackage>
        where TPackage : IPackage
    {
        private readonly IChannel _channel;
        private readonly BridgeObserver<TPackage> _observer;
        private readonly BridgePublisher<TPackage> _publisher;

        private readonly ConcurrentDictionary<ulong, TaskCompletionSource<TPackage>> _taskCompletionSources = new ConcurrentDictionary<ulong, TaskCompletionSource<TPackage>>();
        public ServerBridge(IChannel channel)
        {
            _channel = channel;
            _observer = new BridgeObserver<TPackage>(OnNext);
            _publisher = new BridgePublisher<TPackage>();
        }

        public void OnNext(RedisChannel channel, TPackage package)
        {
            //_channel.Send(package);
        }
    }
}
