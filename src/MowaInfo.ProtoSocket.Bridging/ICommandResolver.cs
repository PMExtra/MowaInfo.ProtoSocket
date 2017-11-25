using System;
using System.Collections.Generic;

namespace MowaInfo.ProtoSocket.Bridging
{
    public interface ICommandResolver
    {
        IEnumerable<Type> CommandsOfMessageType(int messageType);
    }

    public interface ICommandResolver<TObserver> : ICommandResolver
    {
    }
}
