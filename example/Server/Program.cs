using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using DotNetty.Common.Internal.Logging;
using DotNetty.Handlers.Logging;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using Messages;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MowaInfo.ProtoSocket.Abstract;
using MowaInfo.ProtoSocket.Codecs;
using MowaInfo.ProtoSocket.Packing;
using MowaInfo.ProtoSocket.Router;
using MowaInfo.ProtoSocket.Session;
using Server.command;

namespace Server
{
    class Program
    {
        public static IServiceProvider Provider { get; set; }
        public static IServiceCollection Services { get; } = new ServiceCollection();


        private static async Task RunServerAsync()
        {
            ConfigureServices(Services);
            Provider = Services.BuildServiceProvider();
            var loggerFactory = InternalLoggerFactory.DefaultFactory;

            loggerFactory.AddConsole((s, level) => true, false);
            var filters = new List<ICommandFilter>();
            var exceptionHandlers = new List<IExceptionHandler>(); 
            var bossGroup = new MultithreadEventLoopGroup(1);
            var workerGroup = new MultithreadEventLoopGroup();
            var bootstrap = new ServerBootstrap();
            bootstrap
                .Group(bossGroup, workerGroup)
                .Channel<TcpServerSocketChannel>()
                .Option(ChannelOption.SoBacklog, 100)
                //.Handler(new GatewayServer())
                .Handler(new LoggingHandler("SRV-LSTN"))
                .ChildHandler(new ActionChannelInitializer<ISocketChannel>(channel =>
                {
                    var pipeline = channel.Pipeline;
                    pipeline.AddLast(new SessionHandler(new SimpleSessionFactory<GatewaySession>()));
                    pipeline.AddLast(new LoggingHandler("SRV-CONN"));
                    pipeline.AddLast(new ProtobufEncoder<Package>());
                    pipeline.AddLast(new ProtobufDecoder<Package>());
                    pipeline.AddLast(Provider.GetService<MessageSender<Package>>());
                    pipeline.AddLast(new CommandRouter<CommandContext, Package>(Provider, filters, exceptionHandlers));
                    //pipeline.AddLast(new GatewaySetupHandler(Provider.CreateScope()));
                    //pipeline.AddLast(new ChannelHandler(Provider));
                }));

            await bootstrap.BindAsync(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 4433));

            new CancellationTokenSource().Token.WaitHandle.WaitOne();
        }

        static void Main(string[] args)
        {
            RunServerAsync().Wait();
        }

        public static void ConfigureServices(IServiceCollection services)
        {
            //var connectionsSection = GatewayHelper.Configuration.GetSection("Connections");
            //RedisConnection = connectionsSection[nameof(RedisConnection)];
            //services.AddSingleton(_ => new MessageRedisContext(new HostString(RedisConnection)));
            //services.AddSingleton(_ => new EventRedisContext(new HostString(RedisConnection)));
            services.AddCommand(typeof(LoginCommand), typeof(Package));
            services.AddLogging();
            services.AddScoped< MessageSender < IPackage > ,MessageSender <Package>>();
            services.AddScoped<ICommandContext, CommandContext> ();
            services.AddSingleton<IPacker<Package>, SimplePacker<Package>>();
        }
    }
}
