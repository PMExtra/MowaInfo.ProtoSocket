using System.Collections.Concurrent;
using System.Threading.Tasks;
using DotNetty.Transport.Channels;
using MowaInfo.ProtoSocket.Abstract;

namespace MowaInfo.ProtoSocket.Common
{
    public class ReplyManager<T> : SimpleChannelInboundHandler<T>
        where T : IPackage
    {
        private readonly ConcurrentDictionary<ulong, TaskCompletionSource<T>> _taskCompletionSources = new ConcurrentDictionary<ulong, TaskCompletionSource<T>>();

        protected ReplyManager()
        {
        }

        protected override void ChannelRead0(IChannelHandlerContext ctx, T package)
        {

            throw new System.NotImplementedException();
        }
    }
}
