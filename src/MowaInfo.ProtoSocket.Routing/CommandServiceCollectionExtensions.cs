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
        public static IServiceCollection AddCommand(this IServiceCollection services, params Type[] assemblyMarkerTypes)
        {
            return AddCommand(services, assemblyMarkerTypes.Select(t => t.GetTypeInfo().Assembly));
        }

        public static IServiceCollection AddCommand(this IServiceCollection services, IEnumerable<Assembly> assembliesToScan)
        {
            assembliesToScan = assembliesToScan as Assembly[] ?? assembliesToScan.ToArray();
            var allTypes = assembliesToScan
                .SelectMany(a => a.DefinedTypes)
                .ToArray();
            var messageTypes = allTypes
                .Where(m => typeof(IMessage).IsAssignableFrom(m.AsType()))
                .ToArray();
            var commandTypes = allTypes
                .Where(t => messageTypes.Any(m => typeof(ICommand<>).MakeGenericType(m.AsType()).IsAssignableFrom(t.AsType())) && !t.IsAbstract)
                .Select(t => t.AsType())
                .ToArray();
            foreach (var commandType in commandTypes)
            {
                services.AddScoped(commandType);
            }
            services.AddSingleton(_ => new CommandResolver(commandTypes));
            //services.AddSingleton(_ => new CommandResolver(services));
            return services;
        }
    }
}
