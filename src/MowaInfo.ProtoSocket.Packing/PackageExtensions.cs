using MowaInfo.ProtoSocket.Abstract;
using MowaInfo.ProtoSocket.Packing.Internal;

namespace MowaInfo.ProtoSocket.Packing
{
    public static class PackageExtensions
    {
        public static IUpMessage GetMessage(this IPackage package)
        {
            return PackageInfo.GetPackageInfo(package.GetType()).GetMessage(package);
        }

        internal static void SetMessage<T>(this T package, IDownMessage message) where T : IPackage
        {
            PackageInfo<T>.SetMessage(package, message);
        }
    }
}
