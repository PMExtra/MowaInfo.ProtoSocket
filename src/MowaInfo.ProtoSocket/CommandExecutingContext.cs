using DotNetty.Buffers;
using DotNetty.Common.Concurrency;
using DotNetty.Transport.Channels;

namespace MowaInfo.ProtoSocket
{
    public class CommandExecutingContext
    {
        public CommandExecutingContext(IChannelHandlerContext context, object message, object command)
        {
            Cancel = false;
            Channel = context.Channel;
            Allocator = context.Allocator;
            Executor = context.Executor;
            Handler = context.Handler;
            Name = context.Name;
            Message = message;
            Command = command;
        }

        public bool Cancel { get; set; }
        public IChannel Channel { get; }
        public IByteBufferAllocator Allocator { get; }
        public IEventExecutor Executor { get; }
        public IChannelHandler Handler { get; }
        public string Name { get; }
        public object Message { get; }
        public object Command { get; }
    }
}
