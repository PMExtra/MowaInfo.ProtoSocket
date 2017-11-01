using System.IO;
using Messages;
using MowaInfo.RedisContext.Core;
using ProtoBuf;
using StackExchange.Redis;

namespace RedisServer
{
    public class ServerObserver : RedisObserver
    {
        public ServerObserver(RedisChannel channel) : base(channel)
        {
        }

        protected override void OnNext(RedisChannel channel, RedisValue message)
        {
            OnNext(Deserialize<Package>(message));
        }

        protected virtual void OnNext(Package package)
        {
        }
        private static T Deserialize<T>(byte[] bytes)
        {
            using (var stream = new MemoryStream(bytes))
            {
                return Serializer.Deserialize<T>(stream);
            }
        }
    }
}
