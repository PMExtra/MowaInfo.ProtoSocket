using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace MowaInfo.ProtoSocket.Packing
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddMessage(this IServiceCollection services, Assembly assemblyContainMessages)
        {

            return services;
        }
    }
}
