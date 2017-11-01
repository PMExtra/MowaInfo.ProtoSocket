using System;
using System.Diagnostics.CodeAnalysis;

namespace MowaInfo.ProtoSocket.Abstract
{
    public abstract class CommandContextFactory<T> :ICommandContextFactory
        where T : ICommandContext
    {
        [SuppressMessage("ReSharper", "MemberCanBeProtected.Global")]
        public abstract T CreateCommandContext();

        ICommandContext ICommandContextFactory.CreateCommandContext()
        {
            return CreateCommandContext();
        }
    }
}
