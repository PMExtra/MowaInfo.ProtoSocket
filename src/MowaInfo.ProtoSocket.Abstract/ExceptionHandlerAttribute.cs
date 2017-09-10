using System;
using System.Threading.Tasks;

namespace MowaInfo.ProtoSocket.Abstract
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public abstract class ExceptionHandlerAttribute : Attribute, IExceptionHandler
    {
        public int Order { get; set; }

        public abstract Task<bool> HandleException(ICommandContext context, Exception exception);
    }
}
