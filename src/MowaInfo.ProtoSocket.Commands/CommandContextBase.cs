using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using MowaInfo.ProtoSocket.Abstract;

namespace MowaInfo.ProtoSocket.Commands
{
    public abstract class CommandContextBase : ICommandContext
    {
        public IPackage Request { get; set; }

        public IPackage Response { get; set; }

        public IDictionary<object, object> Items { get; set; }

        public IServiceProvider RequestServices { get; set; }

        public CancellationToken RequestAborted { get; set; }

        public ISession Session => RequestServices.GetService<ISession>();

        public virtual void Abort()
        {
            RequestAborted = new CancellationToken(true);
        }

        public abstract TaskCompletionSource<IPackage> Reply(IMessage message);
    }
}
