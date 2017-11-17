using System.Threading.Tasks;

namespace MowaInfo.ProtoSocket.Commands
{
    public interface ICommandFilter
    {
        int Order { get; }

        bool Await { get; }

        Task OnCommandExecuting(ICommandContext context);

        Task OnCommandExecuted(ICommandContext context);
    }
}
