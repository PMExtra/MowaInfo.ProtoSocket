using System.Threading.Tasks;
using DotNetty.Transport.Channels;
using Messages;
using MowaInfo.ProtoSocket.Abstract;

namespace Server
{
    public class CommandContext: CommandContextBase
    {
        private readonly MessageSender<IPackage> _handler;

        public CommandContext(MessageSender<IPackage> handler)
        {
            _handler = handler;
        }

        public override TaskCompletionSource<IPackage> Reply(IMessage message)
        {
            return _handler.Reply(Request.Id, message);

        }
    }
}
