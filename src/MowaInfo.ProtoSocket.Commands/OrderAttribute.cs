using System;
using System.Reflection;

namespace MowaInfo.ProtoSocket.Commands
{
    [AttributeUsage(AttributeTargets.Class)]
    public class OrderAttribute : Attribute
    {
        public OrderAttribute(int order)
        {
            Order = order;
        }

        public int Order { get; }
    }

    public static class OrderExtensions
    {
        public static int GetOrder(this Type type)
        {
#if NETSTANDARD1_3
            return type.GetTypeInfo().GetCustomAttribute<OrderAttribute>()?.Order ?? 0;
#else
            return type.GetCustomAttribute<OrderAttribute>()?.Order ?? 0;
#endif
        }
    }
}
