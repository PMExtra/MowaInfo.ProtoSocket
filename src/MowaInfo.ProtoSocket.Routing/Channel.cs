using System.Threading.Tasks;
using DotNetty.Transport.Channels;
using MowaInfo.ProtoSocket.Abstract;
using IChannel = MowaInfo.ProtoSocket.Abstract.IChannel;

namespace MowaInfo.ProtoSocket.Routing
{
    public class Channel<TPackage> : ChannelHandlerAdapter, IChannel
        where TPackage : IPackage
    {
        protected readonly IPackageNumberer PackageNumberer;

        protected readonly IPacker<TPackage> Packer;

        protected IChannelHandlerContext Context;

        public Channel(IPacker<TPackage> packer, IPackageNumberer packageNumberer)
        {
            Packer = packer;
            PackageNumberer = packageNumberer;
        }

        public virtual TaskCompletionSource<IPackage> Send<T>(T message) where T : IMessage
        {
            var package = Packer.CreatePackage(message);
            return Send(package);
        }

        public virtual TaskCompletionSource<IPackage> Reply<T>(ulong id, T message) where T : IMessage
        {
            var package = Packer.CreatePackage(message);
            package.ReplyId = id;
            return Send(package);
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

        private TaskCompletionSource<IPackage> Send(TPackage package)
        {
            if (package.Id == 0)
            {
                package.Id = PackageNumberer.NextId();
            }

            var source = new TaskCompletionSource<IPackage>();
            Context.WriteAndFlushAsync(package);
            return source;
        }
    }
}
