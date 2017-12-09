using DotNetty.Transport.Channels;
using MowaInfo.ProtoSocket.Abstract;
using IChannel = MowaInfo.ProtoSocket.Abstract.IChannel;

namespace MowaInfo.ProtoSocket.Common
{
    public class Channel<TPackage> : ChannelHandlerAdapter, IChannel
        where TPackage : IPackage
    {
        private readonly ReplyManager<TPackage> _replyManager;
        protected readonly IPackageNumberer PackageNumberer;

        protected readonly IPacker<TPackage> Packer;

        protected IChannelHandlerContext Context;

        public Channel(IPacker<TPackage> packer, IPackageNumberer packageNumberer, ReplyManager<TPackage> replyManager)
        {
            Packer = packer;
            PackageNumberer = packageNumberer;
            _replyManager = replyManager;
        }

        public virtual void Send<T>(T message) where T : IMessage
        {
            var package = Packer.CreatePackage(message);
            Send(package);
        }

        public virtual void Reply<T>(ulong id, T message) where T : IMessage
        {
            var package = Packer.CreatePackage(message);
            package.ReplyId = id;
            Send(package);
        }

        public virtual void Close()
        {
            Context.Channel.CloseAsync();
        }

        public override void ChannelActive(IChannelHandlerContext ctx)
        {
            Context = ctx;
            base.ChannelActive(ctx);
        }

        internal void Send(TPackage package)
        {
            if (package.Id == 0)
            {
                package.Id = PackageNumberer.NextId();
            }

            Context.WriteAndFlushAsync(package);
        }
    }
}
