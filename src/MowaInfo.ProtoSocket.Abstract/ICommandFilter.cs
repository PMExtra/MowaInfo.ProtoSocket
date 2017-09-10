using System.Threading.Tasks;

namespace MowaInfo.ProtoSocket.Abstract
{
    public interface ICommandFilter
    {
        Task OnCommandExecuting(ICommandContext context);

        Task OnCommandExecuted(ICommandContext context);
    }
}
