using System;

namespace MowaInfo.ProtoSocket.Commands
{
    public class SynchronizeAttribute : Attribute
    {
        public SynchronizeAttribute(bool synchronized = true)
        {
            Synchronized = synchronized;
        }

        public bool Synchronized { get; }
    }
}
