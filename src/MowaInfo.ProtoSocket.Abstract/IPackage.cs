using System;

namespace MowaInfo.ProtoSocket.Abstract
{
    public interface IPackage
    {
        ulong Id { get; set; }

        ulong? ReplyId { get; set; }

        int MessageType { get; }

        Type ClassOfMessageType(int messageType);

        int MessageTypeOfClass(Type type);

        object MessageOfMessageType(int messageType);
    }

    public static class PackageDefault
    {
        public static object GetMessage(this IPackage package)
        {
            var messageClass = package.ClassOfMessageType(package.MessageType);
            return MessageOfClass(package, messageClass);
        }

        public static T MessageOfClass<T>(this IPackage package) where T : IMessage
        {
            return (T)MessageOfClass(package, typeof(T));
        }

        public static object MessageOfClass(this IPackage package, Type type)
        {
            var messageType = package.MessageTypeOfClass(type);
            return package.MessageOfMessageType(messageType);
        }
    }
}
