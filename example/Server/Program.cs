using System;
using System.Net;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using DotNetty.Common.Internal.Logging;
using DotNetty.Handlers.Logging;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using Messages;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using MowaInfo.ProtoSocket.Abstract;
using MowaInfo.ProtoSocket.Codecs;
using MowaInfo.ProtoSocket.Routing;
using MowaInfo.ProtoSocket.Session;

namespace Server
{
    internal class Program
    {
        public static IServiceProvider StartUpProvider { get; set; }
        public static IServiceCollection Services { get; } = new ServiceCollection();
        public static IServiceCollection StartUpServices { get; } = new ServiceCollection();


        private static async Task RunServerAsync()
        {
            Startup.ConfigureServices(Services, StartUpServices);
            StartUpProvider = StartUpServices.BuildServiceProvider();
            var loggerFactory = InternalLoggerFactory.DefaultFactory;

            loggerFactory.AddConsole((s, level) => true, false);

            var bossGroup = new MultithreadEventLoopGroup(1);
            var workerGroup = new MultithreadEventLoopGroup();
            var bootstrap = new ServerBootstrap();
            bootstrap
                .Group(bossGroup, workerGroup)
                .Channel<TcpServerSocketChannel>()
                .Option(ChannelOption.SoBacklog, 100)
                .Handler(new LoggingHandler("SRV-LSTN"))
                .ChildHandler(new ActionChannelInitializer<ISocketChannel>(channel =>
                {
                    var startUpProvider = StartUpProvider.CreateScope().ServiceProvider;
                    var pipeline = channel.Pipeline;
                    pipeline.AddLast(new SessionHandler(new SimpleSessionFactory<GatewaySession>()));
                    pipeline.AddLast(new LoggingHandler("SRV-CONN"));
                    pipeline.AddLast(new ProtobufEncoder<Package>());
                    pipeline.AddLast(new ProtobufDecoder<Package>());
                    pipeline.AddLast(startUpProvider.GetService<MessageSender<Package>>());
                    pipeline.AddLast(startUpProvider.GetService<CommandRouter<CommandContext, Package>>());
                }));

            await bootstrap.BindAsync(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 4433));

            new CancellationTokenSource().Token.WaitHandle.WaitOne();
        }

        private static void Main(string[] args)
        {
            //Test.UseStartup<Startup>();
            RunServerAsync().Wait();
        }
    }

    //public static class Test
    //{
    //    public static void UseStartup<TStartup>() where TStartup : class
    //    {
    //        IServiceCollection service = new ServiceCollection();
    //        IServiceCollection startUpService = new ServiceCollection();
    //        var configureServices = typeof(TStartup).GetMethod("ConfigureServices");
    //        var x = Activator.CreateInstance(typeof(TStartup));
    //        configureServices.Invoke(x, new object[] { (IServiceCollection)service, (IServiceCollection)startUpService });

    //    }
    //}
}
