using Generator;
using Messages;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MowaInfo.ProtoSocket.Abstract;
using MowaInfo.ProtoSocket.Bridging;
using MowaInfo.ProtoSocket.Packing;
using MowaInfo.ProtoSocket.Routing;
using Server.command;
using MowaInfo.RedisContext.DependencyInjection;
using RedisServer;

namespace Server
{
    public static class Startup
    {
        public static void ConfigureServices(IServiceCollection services, IServiceCollection startUpServices)
        {
            startUpServices.AddScoped<IPacker<Package>, Packer<Package>>();
            startUpServices.AddScoped<IPackageNumberer, LocalPackageNumberer>();
            startUpServices.AddScoped<MessageSender<Package>>();
            startUpServices.AddScoped<IMessageSender>(s => s.GetService<MessageSender<Package>>());
            startUpServices.AddRouter<CommandContext, Package>();
            startUpServices.AddScoped<ISession, GatewaySession>();
            startUpServices.AddScoped<IServiceCollection>(_ =>
            {
                var collection = new ServiceCollection();
                foreach (var descriptor in services)
                {
                    collection.Add(descriptor);
                }
                return collection;
            });
            startUpServices.AddLogging();

            services.AddCommand(typeof(LoginCommand));
            services.AddRedisContext<BridgeRedisContext>("127.0.0.1:6379");
            services.AddSingleton<ICommandContext, CommandContext>();
            services.AddSingleton<IPackageNumberer, LocalPackageNumberer>();
            services.AddSingleton<IPacker<Package>, Packer<Package>>();
            services.AddSingleton<ApiPublisher>();
        }
    }
}
