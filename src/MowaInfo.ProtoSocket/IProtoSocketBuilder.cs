using Microsoft.Extensions.DependencyInjection;

namespace MowaInfo.ProtoSocket
{
    public interface IProtoSocketBuilder
    {
        IServiceCollection Services { get; }
    }
}
