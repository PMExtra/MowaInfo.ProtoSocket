using System.IO;
using System.Threading.Tasks;
using Messages;
using MowaInfo.RedisContext.Core;
using ProtoBuf;
using StackExchange.Redis;

namespace RedisServer
{
    public class ServerPublish : RedisPublisher
    {
        public ServerPublish(RedisChannel channel) : base(channel)
        {
        }

        public virtual long Publish(Package package)
        {
            return base.Publish(Serialize(package));
        }

        public virtual Task<long> PublishAsync(Package package)
        {
            return base.PublishAsync(Serialize(package));
        }

        private static byte[] Serialize<T>(T instance)
        {
            using (var stream = new MemoryStream())
            {
                Serializer.Serialize(stream, instance);
                return stream.ToArray();
            }
        }
    }
}
