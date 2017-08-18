using System;

namespace MowaInfo.ProtoSocket.Command
{
    /// <summary>Command filter attribute</summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public abstract class CommandFilterAttribute : Attribute
    {
        /// <summary>Gets or sets the execution order.</summary>
        /// <value>The order.</value>
        public int Order { get; set; }

        /// <summary>Called when [command executing].</summary>
        public abstract void OnCommandExecuting(CommandExecutingContext context);

        /// <summary>Called when [command executed].</summary>
        public abstract void OnCommandExecuted(CommandExecutingContext context);
    }
}
