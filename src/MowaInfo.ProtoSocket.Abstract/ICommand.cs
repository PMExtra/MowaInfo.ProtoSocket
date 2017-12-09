using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace MowaInfo.ProtoSocket.Abstract
{
    [SuppressMessage("ReSharper", "TypeParameterCanBeVariant")]
    public interface ICommand<TMessage>
        where TMessage : IMessage
    {
        Task ExecuteAsync(ICommandContext context, TMessage message);
    }
}
