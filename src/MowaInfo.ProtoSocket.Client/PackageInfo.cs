using System;
using MowaInfo.ProtoSocket.Abstract;
using SuperSocket.ProtoBase;

namespace MowaInfo.ProtoSocket.Client
{
    public class PackageInfo<TContainer, TEnum> : IPackageInfo<TEnum>
        where TEnum : struct, IConvertible
        where TContainer : class, IMessageContainer<TEnum>
    {
        public virtual TContainer Body { get; set; }

        public virtual TEnum Key => Body?.Type ?? default(TEnum);
    }
}
