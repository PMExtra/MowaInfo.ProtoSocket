using MowaInfo.ProtoSocket.Abstract;

namespace MowaInfo.ProtoSocket.Commands
{
    public class SimpleCommandContextFactory<T> : CommandContextFactory<T>
        where T : ICommandContext, new()
    {
        public override T CreateCommandContext()
        {
            return new T();
        }
    }
}
