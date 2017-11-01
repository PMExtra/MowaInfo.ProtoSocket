using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MowaInfo.ProtoSocket.Abstract
{
    public abstract class CommandContextBase : ICommandContext
    {
        public IPackage Request { get; set; }

        public IPackage Response { get; set; }

        public IDictionary<object, object> Items { get; set; }

        public IServiceProvider RequestServices { get; set; }

        public CancellationToken RequestAborted { get; set; }

        public ISession Session { get; set; }

        public virtual void Abort()
        {
            RequestAborted = new CancellationToken(true);
        }

        public abstract TaskCompletionSource<IPackage> Reply(IMessage message);
    }
}
