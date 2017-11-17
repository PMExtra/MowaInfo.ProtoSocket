using System.Diagnostics.CodeAnalysis;

namespace MowaInfo.ProtoSocket.Commands
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
