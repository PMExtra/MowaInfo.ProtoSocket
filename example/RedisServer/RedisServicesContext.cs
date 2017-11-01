using Microsoft.AspNetCore.Http;
using MowaInfo.RedisContext.Core;
using StackExchange.Redis;

namespace RedisServer
{
    public class RedisServicesContext :RedisContext
    {
        public RedisServicesContext(HostString host) : base(host)
        {
        }
    }
}
