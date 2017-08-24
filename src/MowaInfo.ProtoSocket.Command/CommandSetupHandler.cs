using System;
using DotNetty.Transport.Channels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace MowaInfo.ProtoSocket.Command
{
    public class CommandSetupHandler : ChannelHandlerAdapter
    {
        public CommandSetupHandler(IServiceScope scope)
        {
            Scope = scope;
            Provider = scope.ServiceProvider;
            Logger = Provider.GetService<ILoggerFactory>().CreateLogger<CommandSetupHandler>();
        }

        public IServiceProvider Provider { get; }

        public ILogger Logger { get; }

        protected IServiceScope Scope { get; }

        public override void ChannelUnregistered(IChannelHandlerContext context)
        {
            Scope?.Dispose();
            base.ChannelUnregistered(context);
        }
    }
}
