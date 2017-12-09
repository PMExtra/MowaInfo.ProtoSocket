using System;

namespace MowaInfo.ProtoSocket.Annotations
{
    [AttributeUsage(AttributeTargets.Class)]
    public class SynchronizeAttribute : Attribute
    {
        public SynchronizeAttribute(bool synchronized = true)
        {
            Synchronized = synchronized;
        }

        public bool Synchronized { get; }
    }
}
