using System;
using System.Threading.Tasks;
using MowaInfo.ProtoSocket.Abstract;
using MowaInfo.RedisContext.Core;

namespace MowaInfo.ProtoSocket.Numberer
{
    public class RedisNumberer : RedisDatabase, IPackageNumberer
    {
        private string _prefix;

        protected virtual string Key { get; set; } = "PackageNumberer";

        public string Prefix
        {
            get => _prefix;
            set
            {
                if (_prefix != null)
                {
                    throw new InvalidOperationException("The prefix cannot be changed at work.");
                }
                _prefix = value ?? throw new ArgumentNullException(nameof(value));
                Key = $"{_prefix}:{Key}";
            }
        }

        public ulong NextId()
        {
            return (ulong)Database.StringIncrement(Key);
        }

        public virtual async Task<ulong> NextIdAsync()
        {
            return (ulong)await Database.StringIncrementAsync(Key);
        }

        public void Reset(ulong starting = 0)
        {
            Database.StringSet(Key, starting);
        }

        public virtual async Task ResetAsync(ulong starting = 0)
        {
            await Database.StringSetAsync(Key, starting);
        }
    }
}
