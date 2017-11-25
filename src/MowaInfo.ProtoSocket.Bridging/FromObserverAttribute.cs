using System;
using MowaInfo.RedisContext.Core;

#if NETSTANDARD1_5
using System.Reflection;
#endif
namespace MowaInfo.ProtoSocket.Bridging
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class FromObserverAttribute : Attribute
    {
        public FromObserverAttribute(Type observerType)
        {
            if (!typeof(RedisObserver).IsAssignableFrom(observerType))
            {
                throw new ArgumentException($"'{nameof(RedisObserver)}' is not assignable from '{observerType.Name}'", nameof(observerType));
            }
            ObserverType = observerType;
        }

        public Type ObserverType { get; }
    }
}
