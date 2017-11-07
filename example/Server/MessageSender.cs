using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using DotNetty.Transport.Channels;
using MowaInfo.ProtoSocket.Abstract;

namespace Server
{
    public class MessageSender<TPackage> : ChannelHandlerAdapter, IMessageSender
        where TPackage : IPackage
    {
        private readonly Hashtable _eventTable = Hashtable.Synchronized(new Hashtable());

        private readonly IPacker<TPackage> _packer;

        private IChannelHandlerContext _context;
        private long _lastMessageId;


        public MessageSender(IPacker<TPackage> packer)
        {
            _packer = packer;
        }

        public Hashtable ReceiveMessageTable { get; set; } = Hashtable.Synchronized(new Hashtable());

        public TaskCompletionSource<IPackage> Send<T>(T message) where T : IMessage
        {
            var package = _packer.CreatePackage(message);
            return Send(package);
        }

        public TaskCompletionSource<IPackage> Reply<T>(ulong id, T message) where T : IMessage
        {
            var package = _packer.CreatePackage(message);
            package.ReplyId = id;
            return Send(package);
        }

        private ulong NextMessageId()
        {
            return (ulong)Interlocked.Increment(ref _lastMessageId);
        }

        public override void ChannelActive(IChannelHandlerContext ctx)
        {
            _context = ctx;
            base.ChannelActive(ctx);
        }

        private TaskCompletionSource<IPackage> Send(TPackage package)
        {
            if (package.Id == 0)
            {
                package.Id = NextMessageId();
            }

            var source = new TaskCompletionSource<IPackage>();
            _eventTable.Add(package.Id, source);
            _context.WriteAndFlushAsync(package);
            return source;
        }
    }
}
