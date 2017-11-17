using System.Threading.Tasks;
using MowaInfo.ProtoSocket.Abstract;

namespace MowaInfo.ProtoSocket.Commands
{
    public abstract class CommandBase<T> : ICommand<T>
        where T : IMessage
    {
        public abstract Task ExecuteAsync(ICommandContext context, T message);
    }
}
