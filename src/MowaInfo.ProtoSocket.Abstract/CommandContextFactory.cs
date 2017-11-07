using System.Diagnostics.CodeAnalysis;

namespace MowaInfo.ProtoSocket.Abstract
{
    public abstract class CommandContextFactory<T> : ICommandContextFactory
        where T : ICommandContext
    {
        ICommandContext ICommandContextFactory.CreateCommandContext()
        {
            return CreateCommandContext();
        }

        [SuppressMessage("ReSharper", "MemberCanBeProtected.Global")]
        public abstract T CreateCommandContext();
    }
}
