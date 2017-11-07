using Microsoft.AspNetCore.Http;
using MowaInfo.RedisContext.Core;

namespace RedisServer
{
    public class RedisServicesContext : RedisContext
    {
        public RedisServicesContext(HostString host) : base(host)
        {
        }
    }
}
