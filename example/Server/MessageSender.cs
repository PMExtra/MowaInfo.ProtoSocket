using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using DotNetty.Transport.Channels;
using MowaInfo.ProtoSocket.Abstract;

namespace Server
{
    public class MessageSender<TPackage> : ChannelHandlerAdapter
        where TPackage : IPackage
    {
        private readonly Hashtable _eventTable = Hashtable.Synchronized(new Hashtable());

        private IChannelHandlerContext _context;
        private long _lastMessageId;

        private readonly IPacker<TPackage> _packer;

        public Hashtable ReceiveMessageTable { get; set; } = Hashtable.Synchronized(new Hashtable());


        public MessageSender(IPacker<TPackage> packer)
        {
            _packer = packer;
        }

        private ulong NextMessageId()
        {
            return (ulong)Interlocked.Increment(ref _lastMessageId);
        }

        public override void ChannelActive(IChannelHandlerContext ctx)
        {
            _context = ctx;
        }

        private TaskCompletionSource<TPackage> Send(TPackage package)
        {
            if (package.Id == 0)
            {
                package.Id = NextMessageId();
            }
            _eventTable.Add(package.Id, new TaskCompletionSource<TPackage>());
            _context.WriteAndFlushAsync(package);
            return _eventTable[package.Id] as TaskCompletionSource<TPackage>;
        }

        public TaskCompletionSource<TPackage> Send<T>(T message) where T : IMessage
        {
            var package = _packer.CreatePackage(message);
            return Send(package);
        }

        public TaskCompletionSource<TPackage> Reply<T>(ulong id, T message) where T : IMessage
        {
            var package = _packer.CreatePackage(message);
            package.ReplyId = id;
            return Send(package);
        }
    }
}
