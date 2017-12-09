using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MowaInfo.ProtoSocket.Abstract
{
    public interface ICommandContext
    {
        IPackage Request { get; set; }

        IPackage Response { get; set; }

        IDictionary<object, object> Items { get; set; }

        IServiceProvider RequestServices { get; set; }

        CancellationToken RequestAborted { get; set; }

        ISession Session { get; }

        void Abort();

        TaskCompletionSource<IPackage> Reply(IMessage message);
    }
}
