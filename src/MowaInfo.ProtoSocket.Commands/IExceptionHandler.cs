using System;
using System.Threading.Tasks;

namespace MowaInfo.ProtoSocket.Commands
{
    public interface IExceptionHandler
    {
        Task<bool> HandleExceptionAsync(ICommandContext commandContext, Exception exception);
    }
}
