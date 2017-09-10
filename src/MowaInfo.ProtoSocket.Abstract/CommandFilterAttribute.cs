using System;
using System.Threading.Tasks;

namespace MowaInfo.ProtoSocket.Abstract
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public abstract class CommandFilterAttribute : Attribute, ICommandFilter
    {
        public int Order { get; set; }

        public abstract Task OnCommandExecuting(ICommandContext context);

        public abstract Task OnCommandExecuted(ICommandContext context);
    }
}
