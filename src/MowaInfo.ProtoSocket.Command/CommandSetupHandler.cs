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
            Provider = scope.ServiceProvider;
            Logger = Provider.GetService<ILoggerFactory>().CreateLogger<CommandSetupHandler>();
        }

        public IServiceProvider Provider { get; }

        public ILogger Logger { get; }
    }
}
