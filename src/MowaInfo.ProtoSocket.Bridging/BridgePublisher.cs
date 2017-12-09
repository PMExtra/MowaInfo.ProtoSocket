using System.IO;
using System.Threading.Tasks;
using MowaInfo.ProtoSocket.Abstract;
using MowaInfo.RedisContext.Core;
using ProtoBuf;

namespace MowaInfo.ProtoSocket.Bridging
{
    public sealed class BridgePublisher<T> : RedisPublisher
        where T : IPackage
    {
        internal BridgePublisher()
        {
        }

        internal long Publish(T package)
        {
            return Publish(Serialize(package));
        }

        internal async Task<long> PublishAsync(T package)
        {
            return await PublishAsync(Serialize(package));
        }

        private static byte[] Serialize(T instance)
        {
            using (var stream = new MemoryStream())
            {
                Serializer.Serialize(stream, instance);
                return stream.ToArray();
            }
        }
    }
}
