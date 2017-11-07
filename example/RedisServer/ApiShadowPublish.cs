using System.IO;
using System.Threading.Tasks;
using Messages;
using ProtoBuf;

namespace RedisServer
{
    public class ApiShadowPublish
    {
        private readonly ApiToServerDatabase _database;
        private readonly ApiObserver _observer;
        private readonly ApiPublish _publish;

        public ApiShadowPublish(ApiToServerDatabase database, ApiPublish publish, ApiObserver observer)
        {
            _database = database;
            _publish = publish;
            _observer = observer;
        }

        public string GetPublishKey(int gatewayId)
        {
            return $"Gateway:{gatewayId}:Publish";
        }

        public string GetStoreKey(int gatewayId, ulong msgId)
        {
            return $"Gateway:{gatewayId}:{msgId}";
        }

        public async Task<long> PublishAsync(int gatewayId, Package msg)
        {
            var buffer = Serialize(msg);
            await _database.StringSetAsync(GetStoreKey(gatewayId, msg.Id), buffer);
            return await _publish.PublishAsync(buffer);
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
