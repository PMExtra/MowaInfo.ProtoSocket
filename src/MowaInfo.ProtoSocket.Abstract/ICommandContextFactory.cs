using System;

namespace MowaInfo.ProtoSocket.Abstract
{
    public interface ICommandContextFactory
    {
        ICommandContext CreateCommandContext();
    }
}
