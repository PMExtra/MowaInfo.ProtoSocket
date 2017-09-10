using System;
using System.Collections.Generic;
using System.Runtime.Remoting.Channels;
using System.Threading;

namespace MowaInfo.ProtoSocket.Abstract
{
    public interface ICommandContext
    {
        IMessageContainer Request { get; set; }
        IMessageContainer Response { get; set; }
        IChannel Channel { get; set; }
        IDictionary<object, object> Items { get; set; }
        IServiceProvider RequestServices { get; set; }
        CancellationToken RequestAborted { get; set; }
        void Abort();
        void Reply(IMessage message);
    }
}
