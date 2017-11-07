using System.Threading.Tasks;
using MowaInfo.ProtoSocket.Abstract;

namespace Server
{
    public class CommandContext : CommandContextBase
    {
        private readonly IMessageSender _handler;

        public CommandContext(IMessageSender handler)
        {
            _handler = handler;
        }

        public override TaskCompletionSource<IPackage> Reply(IMessage message)
        {
            return _handler.Reply(Request.Id, message);
        }
    }
}
