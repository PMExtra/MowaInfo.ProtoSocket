using System;
using System.Threading.Tasks;

namespace MowaInfo.ProtoSocket.Commands
{
    public interface IExceptionHandler
    {
        Task<bool> HandleExceptionAsync(Exception exception);
    }
}
