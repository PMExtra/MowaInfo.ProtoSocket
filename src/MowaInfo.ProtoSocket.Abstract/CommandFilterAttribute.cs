using System;
using System.Threading.Tasks;

namespace MowaInfo.ProtoSocket.Abstract
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public abstract class CommandFilterAttribute : Attribute
    {
        public int Order { get; set; }

        public virtual bool Await { get; protected set; } = true;

        public abstract Task OnCommandExecuting(ICommandContext context);

        public abstract Task OnCommandExecuted(ICommandContext context);
    }
}
