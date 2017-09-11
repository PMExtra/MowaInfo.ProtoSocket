using System;
using System.Linq;
using System.Reflection;
using MowaInfo.ProtoSocket.Abstract;

namespace MowaInfo.ProtoSocket
{
    public static class ProtoSocketBuilderExtensions
    {
        public static IProtoSocketBuilder UseStartup<T>(this IProtoSocketBuilder builder)
        {
            return builder.UseStartup(typeof(T));
        }

        public static IProtoSocketBuilder UseCommands(this IProtoSocketBuilder builder, params Type[] commandTypes)
        {
            return builder.UseCommands(commandTypes.AsEnumerable());
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
            return builder.UseCommands(commandTypes);
        }
    }
}
