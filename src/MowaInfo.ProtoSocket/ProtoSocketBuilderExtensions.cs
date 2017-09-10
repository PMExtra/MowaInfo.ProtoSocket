using System;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using MowaInfo.ProtoSocket.Abstract;

namespace MowaInfo.ProtoSocket
{
    public static class ProtoSocketBuilderExtensions
    {
        public static IProtoSocketBuilder UseCommands(this IProtoSocketBuilder builder, params Type[] commandTypes)
        {
            foreach (var type in commandTypes)
            {
                builder.Services.AddTransient(type);
            }
            return builder;
        }

        public static IProtoSocketBuilder UseCommandsIn(this IProtoSocketBuilder builder, Assembly assembly)
        {
#if NETSTANDARD1_3
            var commandTypes = assembly.ExportedTypes.Where(type => type.GetInterfaces()
                .Any(@interface => @interface.GetTypeInfo().IsGenericType && @interface.GetGenericTypeDefinition() == typeof(ICommand<>)));
#else
            var commandTypes = assembly.ExportedTypes.Where(type => type.GetInterfaces()
                .Any(@interface => @interface.IsGenericType && @interface.GetGenericTypeDefinition() == typeof(ICommand<>)));
#endif
            return UseCommands(builder, commandTypes.ToArray());
        }
    }
}
