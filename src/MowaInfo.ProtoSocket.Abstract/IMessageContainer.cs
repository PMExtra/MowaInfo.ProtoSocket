using System;

namespace MowaInfo.ProtoSocket.Abstract
{
    public interface IMessageContainer
    {
        ulong Id { get; set; }

        ulong? ReplyId { get; set; }

        int MessageType { get; }

        Type ClassOfMessageType(int messageType);

        int MessageTypeOfClass(Type type);

        object MessageOfMessageType(int messageType);
    }

    public static class MessageContainerDefault
    {
        public static object GetMessage(this IMessageContainer container)
        {
            var messageClass = container.ClassOfMessageType(container.MessageType);
            return MessageOfClass(container, messageClass);
        }

        public static T MessageOfClass<T>(this IMessageContainer container) where T : IMessage
        {
            return (T)MessageOfClass(container, typeof(T));
        }

        public static object MessageOfClass(this IMessageContainer container, Type type)
        {
            var messageType = container.MessageTypeOfClass(type);
            return container.MessageOfMessageType(messageType);
        }
    }
}
