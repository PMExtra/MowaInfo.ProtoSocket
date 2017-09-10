using System;
using System.Collections.Generic;
using System.Threading;

namespace MowaInfo.ProtoSocket.Abstract
{
    public abstract class CommandContextBase : ICommandContext
    {
        public IMessageContainer Request { get; set; }

        public IMessageContainer Response { get; set; }

        public IDictionary<object, object> Items { get; set; }

        public IServiceProvider RequestServices { get; set; }

        public CancellationToken RequestAborted { get; set; }

        public ISession Session { get; set; }

        public virtual void Abort()
        {
            RequestAborted = new CancellationToken(true);
        }

        public abstract void Reply(IMessage message);
    }
}
