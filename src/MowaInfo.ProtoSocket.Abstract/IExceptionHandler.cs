using System;
using System.Threading.Tasks;

namespace MowaInfo.ProtoSocket.Abstract
{
    public interface IExceptionHandler
    {
        Task<bool> HandleExceptionAsync(ICommandContext commandContext, Exception exception);
    }
}
