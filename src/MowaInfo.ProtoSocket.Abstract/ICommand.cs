using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace MowaInfo.ProtoSocket.Abstract
{
    public interface ICommand<TMessage> : ICommand<ICommandContext, TMessage>
        where TMessage : IMessage
    {
    }

    [SuppressMessage("ReSharper", "TypeParameterCanBeVariant")]
    public interface ICommand<TContext, TMessage>
        where TContext : ICommandContext
        where TMessage : IMessage
    {
        Task ExecuteAsync(TContext context, TMessage message);
    }
}
