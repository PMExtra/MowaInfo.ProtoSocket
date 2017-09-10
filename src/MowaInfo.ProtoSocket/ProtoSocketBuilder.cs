using Microsoft.Extensions.DependencyInjection;

namespace MowaInfo.ProtoSocket
{
    internal class ProtoSocketBuilder : IProtoSocketBuilder
    {
        public ProtoSocketBuilder(IServiceCollection services)
        {
            Services = services;
        }

        public IServiceCollection Services { get; }
    }
}
