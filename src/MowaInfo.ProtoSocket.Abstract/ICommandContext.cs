using System;
using System.Collections.Generic;
using System.Threading;

namespace MowaInfo.ProtoSocket.Abstract
{
    public interface ICommandContext
    {
        IMessageContainer Request { get; set; }

        IMessageContainer Response { get; set; }
        
        IDictionary<object, object> Items { get; set; }

        IServiceProvider RequestServices { get; set; }

        CancellationToken RequestAborted { get; set; }

        ISession Session { get; set; }

        void Abort();

        void Reply(IMessage message);
    }
}
