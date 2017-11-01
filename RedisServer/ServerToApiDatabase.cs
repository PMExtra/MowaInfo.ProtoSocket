using MowaInfo.RedisContext.Annotations;
using MowaInfo.RedisContext.Core;

namespace RedisServer
{
    [GetDatabase(2)]
    public class ServerToApiDatabase : RedisDatabase
    {
        
    }
}
