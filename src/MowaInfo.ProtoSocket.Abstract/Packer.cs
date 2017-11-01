using System;
using System.Diagnostics.CodeAnalysis;

namespace MowaInfo.ProtoSocket.Abstract
{
    public abstract class Packer<T> : IPacker<T>
        where T : IPackage
    {
        [SuppressMessage("ReSharper", "MemberCanBeProtected.Global")]
        public abstract T CreatePackage(IMessage message);

        [SuppressMessage("ReSharper", "MemberCanBeProtected.Global")]
        public abstract T CreatePackage(int messageType);

        T IPacker<T>.CreatePackage(IMessage message)
        {
            return CreatePackage(message);
        }

        T IPacker<T>.CreatePackage(int messageType)
        {
            return CreatePackage(messageType);
        }
    }
}
