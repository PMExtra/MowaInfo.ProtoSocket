using System;
using System.Threading.Tasks;

namespace MowaInfo.ProtoSocket.Abstract
{
    public interface IExceptionHandler
    {
        Task<bool> HandleException(ICommandContext context, Exception exception);
    }
}
