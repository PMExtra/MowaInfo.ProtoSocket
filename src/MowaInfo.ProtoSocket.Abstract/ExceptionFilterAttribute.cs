using System;
using System.Threading.Tasks;

namespace MowaInfo.ProtoSocket.Abstract
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public abstract class ExceptionFilterAttribute : Attribute
    {
        public int Order { get; set; }

        public virtual Task OnExceptionAsync(ICommandContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }
            OnException(context);
            return Task.CompletedTask;
        }

        public abstract void OnException(ICommandContext context);
    }
}
