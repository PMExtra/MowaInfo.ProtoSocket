using System.Threading.Tasks;

namespace MowaInfo.ProtoSocket.Abstract
{
    public interface ICommandFilter
    {
        int Order { get; }

        bool Await { get; }

        Task OnCommandExecuting(ICommandContext context);

        Task OnCommandExecuted(ICommandContext context);
    }
}
