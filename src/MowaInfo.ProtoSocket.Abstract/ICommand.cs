using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace MowaInfo.ProtoSocket.Abstract
{
    [SuppressMessage("ReSharper", "TypeParameterCanBeVariant")]
    public interface ICommand<TContext, TMessage>
        where TMessage : IMessage
    {
        Task ExecuteCommandAsync(TContext context, TMessage message);
    }
}
