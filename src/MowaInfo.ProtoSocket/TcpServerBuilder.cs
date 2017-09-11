using System;
using System.Collections.Generic;
using System.Security.Authentication;
using System.Threading.Tasks;
using DotNetty.Handlers.Logging;
using DotNetty.Handlers.Tls;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using Microsoft.Extensions.DependencyInjection;
using MowaInfo.ProtoSocket.Abstract;
using MowaInfo.ProtoSocket.Codecs;

namespace MowaInfo.ProtoSocket
{
    public class TcpServerBuilder : IProtoSocketBuilder
    {
        private TcpServerBuilder()
        {
        }

        public IServiceCollection Services { get; } = new ServiceCollection();

        public IProtoSocketBuilder UseStartup(Type typeOfStartup)
        {
            throw new NotImplementedException();
        }

        public IProtoSocketBuilder UseCommands(IEnumerable<Type> commandTypes)
        {
            throw new NotImplementedException();
        }

        public IServer Build()
        {
            var bossGroup = new MultithreadEventLoopGroup(1);
            var workerGroup = new MultithreadEventLoopGroup();
            var bootstrap = new ServerBootstrap();
            bootstrap
                .Group(bossGroup, workerGroup)
                .Channel<TcpServerSocketChannel>()
                .Handler(new LoggingHandler("SRV-LSTN"))
                .ChildHandler(new ActionChannelInitializer<ISocketChannel>(channel =>
                {
                    var pipeline = channel.Pipeline;
                    if (GatewayServer.SslProtocol != SslProtocols.None)
                    {
                        var settings = new ServerTlsSettings(
                            GatewayServer.Certificate,
                            GatewayServer.NegotiateClientCertificate,
                            GatewayServer.CheckCertificateRevocation,
                            GatewayServer.SslProtocol);
                        pipeline.AddLast("tls", new TlsHandler(settings));
                    }
                    pipeline.AddLast(new LoggingHandler("SRV-CONN"));
                    pipeline.AddLast(new ProtobufEncoder<MessageContainer>());
                    pipeline.AddLast(new ProtobufDecoder<MessageContainer>());
                    pipeline.AddLast(new GatewaySetupHandler(Provider.CreateScope()));
                    pipeline.AddLast(new ClientCertHandler());
                    pipeline.AddLast(new GatewayCommandRouter(Services));
                }));
        }

        public static IProtoSocketBuilder DefaultBuilder()
        {
            return new TcpServerBuilder();
        }
    }
}
