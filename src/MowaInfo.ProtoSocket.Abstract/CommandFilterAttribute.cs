using System;

namespace MowaInfo.ProtoSocket.Abstract
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public abstract class CommandFilterAttribute : Attribute
    {
        public int Order { get; set; }

        public abstract void OnCommandExecuting(ICommandContext context);

        public abstract void OnCommandExecuted(ICommandContext context);
    }
}
