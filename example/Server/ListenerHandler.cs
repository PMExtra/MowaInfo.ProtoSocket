using DotNetty.Common.Utilities;
using DotNetty.Transport.Channels;
using MowaInfo.ProtoSocket.Abstract;

namespace Server
{
    public class ListenerHandler : ChannelHandlerAdapter
    {
        private ISession _session;

        public override void ChannelActive(IChannelHandlerContext ctx)
        {
            _session = ctx.Channel.GetAttribute(AttributeKey<ISession>.ValueOf(nameof(_session))).Get();
        }
    }
}
