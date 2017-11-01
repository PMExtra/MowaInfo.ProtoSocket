using System;
using MowaInfo.ProtoSocket.Abstract;

namespace MowaInfo.ProtoSocket.Packing
{
    public class SimplePacker<T> : Packer<T>
        where T : IPackage, new()
    {
        public override T CreatePackage(IMessage message)
        {
            return (T)Activator.CreateInstance(typeof(T), message);
        }
        public override T CreatePackage(int messageType)
        {
            return (T)Activator.CreateInstance(typeof(T), messageType);
        }
    }
}
