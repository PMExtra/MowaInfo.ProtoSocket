namespace MowaInfo.ProtoSocket.Abstract
{
    public interface IPackage
    {
        ulong Id { get; set; }

        ulong? ReplyId { get; set; }

        int MessageType { get; set; }
    }

    public static class PackageDefault
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
