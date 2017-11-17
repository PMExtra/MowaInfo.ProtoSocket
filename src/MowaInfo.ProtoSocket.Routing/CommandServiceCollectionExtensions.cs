using System;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using MowaInfo.ProtoSocket.Abstract;
using MowaInfo.ProtoSocket.Commands;

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

        public static IServiceCollection AddCommands(this IServiceCollection services, params Assembly[] assembliesToScan)
        {
#if NETSTANDARD1_3
            var commandTypes = assembliesToScan
                .SelectMany(assembly => assembly.ExportedTypes.Where(type => type.GetInterfaces()
                    .Any(@interface => @interface.GetTypeInfo().IsGenericType && @interface.GetGenericTypeDefinition() == typeof(ICommand<>))))
                .ToArray();
#else
            var commandTypes = assembliesToScan
                .SelectMany(assembly => assembly.ExportedTypes.Where(type => type.GetInterfaces()
                    .Any(@interface => @interface.IsGenericType && @interface.GetGenericTypeDefinition() == typeof(ICommand<>))))
                .ToArray();
#endif
            foreach (var commandType in commandTypes)
            {
                services.AddScoped(commandType);
            }
            services.AddSingleton(new CommandResolver(commandTypes));
            return services;
        }

        public static IServiceCollection AddExceptionHandler(this IServiceCollection services, Type handlerType)
        {
            services.AddScoped(typeof(IExceptionHandler), handlerType);
            return services;
        }

        public static IServiceCollection AddExceptionHandlers(this IServiceCollection services, params Type[] handlerTypes)
        {
            foreach (var handler in handlerTypes)
            {
                services.AddScoped(handler);
            }
            return services;
        }

        public static IServiceCollection AddExceptionHandlers(this IServiceCollection services, params Assembly[] assembliesToScan)
        {
            var handlers = assembliesToScan
                .SelectMany(assembly => assembly.ExportedTypes.Where(type => typeof(IExceptionHandler).IsAssignableFrom(type)))
                .ToArray();
            foreach (var handler in handlers)
            {
                services.AddScoped(handler);
            }
            return services;
        }
    }
}
