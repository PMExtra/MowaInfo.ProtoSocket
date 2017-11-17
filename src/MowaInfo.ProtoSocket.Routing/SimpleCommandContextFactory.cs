using MowaInfo.ProtoSocket.Commands;

namespace MowaInfo.ProtoSocket.Routing
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
