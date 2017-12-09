using System;
using System.IO;
using MowaInfo.ProtoSocket.Abstract;
using MowaInfo.RedisContext.Core;
using ProtoBuf;
using StackExchange.Redis;

namespace MowaInfo.ProtoSocket.Bridging
{
    public sealed class BridgeObserver<T> : RedisObserver
        where T : IPackage 
    {
        private readonly Action<RedisChannel, T> _onNext;

        internal BridgeObserver(Action<RedisChannel, T> onNext)
        {
            _onNext = onNext;
        }

        protected override void OnNext(RedisChannel channel, RedisValue message)
        {
            var package = Deserialize(message);
            _onNext?.Invoke(channel, package);
        }
        
        private static T Deserialize(byte[] bytes)
        {
            using (var stream = new MemoryStream(bytes))
            {
                return Serializer.Deserialize<T>(stream);
            }
        }
    }
}
