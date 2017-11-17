using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using MowaInfo.ProtoSocket.Abstract;

namespace MowaInfo.ProtoSocket.Commands
{
    [SuppressMessage("ReSharper", "TypeParameterCanBeVariant")]
    public interface ICommand<TMessage>
        where TMessage : IMessage
    {
        Task ExecuteAsync(ICommandContext context, TMessage message);
    }
}
