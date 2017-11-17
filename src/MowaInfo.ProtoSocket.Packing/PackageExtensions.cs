using MowaInfo.ProtoSocket.Abstract;
using MowaInfo.ProtoSocket.Packing.Internal;

namespace MowaInfo.ProtoSocket.Packing
{
    public static class PackageExtensions
    {
        public static IMessage GetMessage<T>(this T package) where T : IPackage
        {
            return PackageInfo<T>.GetMessage(package);
        }

        public static void SetMessage<T>(this T package, IMessage message) where T : IPackage
        {
            PackageInfo<T>.SetMessage(package, message);
        }
    }
}
