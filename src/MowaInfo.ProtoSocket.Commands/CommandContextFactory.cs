using System.Diagnostics.CodeAnalysis;
using MowaInfo.ProtoSocket.Abstract;

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
