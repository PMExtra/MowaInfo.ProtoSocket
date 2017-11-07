using DotNetty.Common.Utilities;
using DotNetty.Transport.Channels;
using MowaInfo.ProtoSocket.Abstract;

namespace MowaInfo.ProtoSocket.Session
{
    public class SessionHandler : ChannelHandlerAdapter
    {
        private readonly ISession _session;

        public SessionHandler(ISessionFactory factory)
        {
            _session = factory.CreateSession();
        }

        public override void ChannelActive(IChannelHandlerContext context)
        {
            context.Channel.GetAttribute(AttributeKey<ISession>.ValueOf(nameof(_session))).SetIfAbsent(_session);
            base.ChannelActive(context);
        }

        public override void ChannelInactive(IChannelHandlerContext context)
        {
            context.Channel.GetAttribute(AttributeKey<ISession>.ValueOf(nameof(_session))).Remove();
            base.ChannelActive(context);
        }
    }
}
