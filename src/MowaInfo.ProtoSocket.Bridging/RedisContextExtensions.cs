using MowaInfo.ProtoSocket.Abstract;

namespace MowaInfo.ProtoSocket.Bridging
{
    public static class RedisContextExtensions
    {
        public static void AddApiBridge<TBridge, TPackage>(this RedisContext.Core.RedisContext context, TBridge bridge)
            where TBridge : ApiBridge<TPackage>
            where TPackage : IPackage
        {
            context.AddPublisher(bridge.Publisher);
            context.AddObserver(bridge.Observer);
        }
    }
}
