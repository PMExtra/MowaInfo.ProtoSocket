using Microsoft.Extensions.DependencyInjection;

namespace MowaInfo.ProtoSocket
{
    public static class ServiceCollectionExtensions
    {
        public static IProtoSocketBuilder AddProtoSocket(this IServiceCollection services)
        {
            return new ProtoSocketBuilder(services);
        }
    }
}
