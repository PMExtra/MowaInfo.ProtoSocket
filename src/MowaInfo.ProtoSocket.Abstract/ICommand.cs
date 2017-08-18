namespace MowaInfo.ProtoSocket.Abstract
{
    /// <summary>
    ///     Command basic interface
    /// </summary>
    public interface ICommand<in TContext, in TMessage> : ICommandBase
    {
        /// <summary>
        ///     Gets the name.
        /// </summary>
        string Name { get; }

        void ExecuteCommand(TContext context, TMessage message);
    }

    public interface ICommandBase
    {
    }
}
