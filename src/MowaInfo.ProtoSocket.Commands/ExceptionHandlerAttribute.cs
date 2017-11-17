using System;
using System.Threading.Tasks;

namespace MowaInfo.ProtoSocket.Commands
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public abstract class ExceptionHandlerAttribute : Attribute, IExceptionHandler
    {
        public int Order { get; set; }

        public abstract Task<bool> HandleExceptionAsync(Exception exception);
    }
}
