using Messages;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MowaInfo.ProtoSocket.Abstract;
using MowaInfo.ProtoSocket.Packing;
using MowaInfo.ProtoSocket.Routing;
using Server.command;

namespace Server
{
    public static class Startup
    {
        public static void ConfigureServices(IServiceCollection services, IServiceCollection startUpServices)
        {
            startUpServices.AddScoped<IPacker<Package>, SimplePacker<Package>>();
            startUpServices.AddScoped<MessageSender<Package>>();
            startUpServices.AddScoped<IMessageSender>(s => s.GetService<MessageSender<Package>>());
            //startUpServices.AddScoped<CommandRouter<CommandContext, Package>>();
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

            services.AddCommand(typeof(LoginCommand), typeof(Package));
            services.AddScoped<ICommandContext, CommandContext>();
            services.AddLogging();
        }
    }
}
