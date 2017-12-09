using System;

namespace MowaInfo.ProtoSocket.Annotations
{
    [AttributeUsage(AttributeTargets.Class)]
    public class MessageTypeAttribute : Attribute
    {
        public MessageTypeAttribute(int messageType)
        {
            MessageType = messageType;
        }

        public int MessageType { get; }
    }
}
