using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using MowaInfo.ProtoSocket.Abstract;

namespace MowaInfo.ProtoSocket
{
    public interface IProtoSocketBuilder
    {
        IServiceCollection Services { get; }

        IProtoSocketBuilder UseStartup(Type typeOfStartup);

        IProtoSocketBuilder UseCommands(IEnumerable<Type> commandTypes);

        IProtoSocketBuilder UsePackage(Type typeOfPackage);

        IServer Build();
    }
}
