using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MowaInfo.ProtoSocket.Abstract
{
    public class InMemorySession : ISession
    {
        private readonly Dictionary<string, byte[]> _store = new Dictionary<string, byte[]>();

        public bool IsAvailable => true;

        public Guid Id { get; } = Guid.NewGuid();

        public IEnumerable<string> Keys => _store.Keys;

        public Task LoadAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
#if NET451
            return Task.FromResult(0);
#else
            return Task.CompletedTask;
#endif
        }

        public Task CommitAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
#if NET451
            return Task.FromResult(0);
#else
            return Task.CompletedTask;
#endif
        }

        public bool TryGetValue(string key, out byte[] value)
        {
            return _store.TryGetValue(key, out value);
        }

        public void Set(string key, byte[] value)
        {
            _store[key] = value;
        }

        public void Remove(string key)
        {
            _store.Remove(key);
        }

        public void Clear()
        {
            _store.Clear();
        }
    }
}
