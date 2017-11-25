using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace MowaInfo.ProtoSocket.Bridging
{
    public static class ServicesCollectionExtensions
    {
        public static IServiceCollection AddBridgeCommands(this IServiceCollection services, params Assembly[] assembliesToScan)
        {
#if NETSTANDARD1_5
            var commandTypes = assembliesToScan
                .SelectMany(assembly => assembly.ExportedTypes.Where(type => type.GetInterfaces()
                    .Any(@interface => @interface.GetTypeInfo().IsGenericType && @interface.GetGenericTypeDefinition() == typeof(IBridgeCommand<>))))
                .ToArray();
#else
            var commandTypes = assembliesToScan
                .SelectMany(assembly => assembly.ExportedTypes.Where(type => type.GetInterfaces()
                    .Any(@interface => @interface.IsGenericType && @interface.GetGenericTypeDefinition() == typeof(IBridgeCommand<>))))
                .ToArray();
#endif
            foreach (var commandType in commandTypes)
            {
                services.AddScoped(commandType);
            }
            services.AddSingleton(new CommandResolverFactory(commandTypes));
            services.AddTransient(typeof(ICommandResolver<>), typeof(CommandResolver<>));
            return services;
        }
    }
}
