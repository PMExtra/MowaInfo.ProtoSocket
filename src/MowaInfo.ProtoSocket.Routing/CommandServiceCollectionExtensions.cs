using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using MowaInfo.ProtoSocket.Abstract;

namespace MowaInfo.ProtoSocket.Routing
{
    public static class CommandServiceCollectionExtensions
    {
        public static IServiceCollection AddRouter<TCommandContext, TPackage>(this IServiceCollection services)
            where TCommandContext : ICommandContext
            where TPackage : IPackage
        {
            return services.AddScoped<CommandRouter<TCommandContext, TPackage>>();
        }

        public static IServiceCollection AddCommand(this IServiceCollection services, Type assemblyMarkerType)
        {
            return AddCommand(services, assemblyMarkerType.GetTypeInfo().Assembly);
        }

        public static IServiceCollection AddCommand(this IServiceCollection services, Assembly assembliesToScan)
        {
#if NETSTANDARD1_3
            var commandTypes = assembliesToScan.ExportedTypes.Where(type => type.GetInterfaces()
                .Any(@interface => @interface.GetTypeInfo().IsGenericType && @interface.GetGenericTypeDefinition() == typeof(ICommand<>)));
#else
            var commandTypes = assembliesToScan.ExportedTypes.Where(type => type.GetInterfaces()
                .Any(@interface => @interface.IsGenericType && @interface.GetGenericTypeDefinition() == typeof(ICommand<>)));
#endif
            var enumerable = commandTypes as Type[] ?? commandTypes.ToArray();
            foreach (var commandType in enumerable)
            {
                services.AddScoped(commandType);
            }
            services.AddScoped(_ => new CommandResolver(enumerable));
            return services;
        }
    }
}
