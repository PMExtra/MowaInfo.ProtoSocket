using System;
using System.Collections;
using System.Threading.Tasks;
using DotNetty.Transport.Channels;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MowaInfo.ProtoSocket.Abstract;
using MowaInfo.ProtoSocket.Router;

namespace Server
{
    public class GatewayServer : ChannelHandlerAdapter, IServer
    {
        public IServiceProvider ServiceProvider;
        //public readonly string RedisConnection;

        public GatewayServer()
        {
            //Configuration = new ConfigurationBuilder()
            //    .SetBasePath(Environment.CurrentDirectory)
            //    .AddJsonFile("appsettings.json", false, true).Build();
            //var connectionsSection = Configuration.GetSection("Connections");
            //RedisConnection = connectionsSection[nameof(RedisConnection)];
        }

        public Hashtable GatewaySesssionTable { get; set; }
        public IConfiguration Configuration { get; }

        public Task StartAsync()
        {
            var services = new ServiceCollection();
            services.AddLogging();
            //services.AddTransient(_ => new EventRedisContext(new HostString(RedisConnection)));
            //services.AddTransient(_ => new MessageRedisContext(new HostString(RedisConnection)));
            var serviceProviderFactory = new DefaultServiceProviderFactory();
            ServiceProvider = serviceProviderFactory.CreateServiceProvider(services);
            GatewaySesssionTable = Hashtable.Synchronized(new Hashtable());
            
            return Task.CompletedTask;
        }

        public Task StopAsync()
        {
            return Task.CompletedTask;
        }

        public override void ChannelRegistered(IChannelHandlerContext ctx)
        {
            StartAsync().Wait();
            ctx.FireChannelRegistered();
        }

        public override void ChannelUnregistered(IChannelHandlerContext ctx)
        {
            StopAsync().Wait();
            ctx.FireChannelUnregistered();
        }
    }
}
