using System;
using MowaInfo.ProtoSocket.Abstract;

namespace MowaInfo.ProtoSocket.Packing
{
    public class Packer : IPacker
    {
        public Packer(Type upPackageType, Type downPackageType)
        {
            UpPackageType = upPackageType;
            DownPackageType = downPackageType;
        }

        public Type UpPackageType { get; }

        public Type DownPackageType { get; }
        
        public IPackage CreatePackage(IDownMessage message)
        {
            var package = (IPackage)Activator.CreateInstance(DownPackageType);
            package.SetMessage(message);
            return package;
        }
    }
}
