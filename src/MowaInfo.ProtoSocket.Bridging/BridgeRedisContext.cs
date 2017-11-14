using Microsoft.AspNetCore.Http;

namespace MowaInfo.ProtoSocket.Bridging
{
    public class BridgeRedisContext : RedisContext.Core.RedisContext
    {
        public BridgeRedisContext(HostString host) : base(host)
        {
        }
    }
}
