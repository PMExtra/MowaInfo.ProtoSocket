namespace MowaInfo.ProtoSocket.Abstract
{
    /// <summary>
    ///     Command basic interface
    /// </summary>
    public abstract class CommandBase<TContext, TMessage> : ICommandBase
    {
        /// <summary>
        ///     Gets the name.
        /// </summary>
        public static string Name => throw new System.NotImplementedException();

        public abstract void ExecuteCommand(TContext context, TMessage message);
    }

    public interface ICommandBase
    {
    }
}
