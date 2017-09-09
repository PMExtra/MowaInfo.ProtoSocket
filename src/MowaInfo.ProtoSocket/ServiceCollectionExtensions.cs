using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using MowaInfo.ProtoSocket.Abstract;

namespace MowaInfo.ProtoSocket
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddProtoSocketCommand(this IServiceCollection services, Assembly assembly)
        {
            var types = assembly.ExportedTypes.Where(type => type.GetInterfaces()
                .Any(@interface => @interface.IsGenericType && @interface.GetGenericTypeDefinition() == typeof(ICommand<>)));
            foreach (var type in types)
            {
                services.AddTransient(type);
            }
            return services;
        }
    }
}
