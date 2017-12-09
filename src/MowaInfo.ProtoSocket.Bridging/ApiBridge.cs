using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading.Tasks;
using MowaInfo.Promises;
using MowaInfo.ProtoSocket.Abstract;
using StackExchange.Redis;

namespace MowaInfo.ProtoSocket.Bridging
{
    public class ApiBridge<TPackage>
        where TPackage : IPackage
    {
        private readonly IPackageNumberer _numberer;
        private readonly IPacker<TPackage> _packer;
        private readonly ConcurrentDictionary<ulong, Promise<TPackage>> _taskCompletionSources = new ConcurrentDictionary<ulong, Promise<TPackage>>();

        public ApiBridge(IPacker<TPackage> packer, IPackageNumberer numberer)
        {
            _packer = packer;
            _numberer = numberer;
            Publisher = new BridgePublisher<TPackage>();
            Observer = new BridgeObserver<TPackage>(OnNext);
        }

        public BridgePublisher<TPackage> Publisher { get; }

        public BridgeObserver<TPackage> Observer { get; }

        public Task<TPackage> Request(IMessage message, int seconds = 60)
        {
            var package = _packer.CreatePackage(message);
            package.Id = _numberer.NextId();
            var source = new Promise<TPackage>();
            source.SetTimeout(seconds * 1000, TimeOrigin.Current);
            _taskCompletionSources[package.Id] = source;
            Publisher.Publish(package);
            return source.Task.ContinueWith(task =>
            {
                _taskCompletionSources.TryRemove(package.Id, out var _);
                return task.Result;
            });
        }

        public async Task<Task<TPackage>> RequestAsync(IMessage message, int seconds = 60)
        {
            var package = _packer.CreatePackage(message);
            package.Id = await _numberer.NextIdAsync();
            var source = new Promise<TPackage>();
            source.SetTimeout(seconds * 1000, TimeOrigin.Current);
            _taskCompletionSources[package.Id] = source;
            await Publisher.PublishAsync(package);
            return source.Task.ContinueWith(task =>
            {
                _taskCompletionSources.TryRemove(package.Id, out var _);
                return task.Result;
            });
        }

        public void Send(IMessage message)
        {
            var package = _packer.CreatePackage(message);
            package.Id = _numberer.NextId();
            Publisher.Publish(package);
        }

        public async Task SendAsync(IMessage message)
        {
            var package = _packer.CreatePackage(message);
            package.Id = await _numberer.NextIdAsync();
            await Publisher.PublishAsync(package);
        }

        public void OnNext(RedisChannel channel, TPackage package)
        {
            Debug.Assert(package.ReplyId != null, "package.ReplyId != null");
            if (!_taskCompletionSources.TryRemove(package.ReplyId.Value, out var source))
            {
                throw new ArgumentOutOfRangeException(nameof(package.ReplyId));
            }

            source.SetResult(package);
        }
    }
}
