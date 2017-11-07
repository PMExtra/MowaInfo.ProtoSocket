using System;
using System.Threading.Tasks;
using MowaInfo.RedisContext.Annotations;
using MowaInfo.RedisContext.Core;
using StackExchange.Redis;

namespace RedisServer
{
    [GetDatabase(1)]
    public class ApiToServerDatabase : RedisDatabase
    {
        public string StringGet(string key)
        {
            return Database.StringGet(key).ToString();
        }

        public bool StringSet(string key, RedisValue value)
        {
            return Database.StringSet(key, value);
        }

        public bool StringSet(string key, RedisValue value, TimeSpan expiry)
        {
            return Database.StringSet(key, value, expiry);
        }

        public bool KeyDelete(string key)
        {
            return Database.KeyDelete(key);
        }

        public async Task<string> StringGetAsync(string key)
        {
            return (await Database.StringGetAsync(key)).ToString();
        }

        public async Task<bool> StringSetAsync(string key, RedisValue value)
        {
            return await Database.StringSetAsync(key, value);
        }

        public async Task<bool> StringSetAsync(string key, RedisValue value, TimeSpan expiry)
        {
            return await Database.StringSetAsync(key, value, expiry);
        }

        public async Task<bool> KeyDeleteAsync(string key)
        {
            return await Database.KeyDeleteAsync(key);
        }
    }
}
