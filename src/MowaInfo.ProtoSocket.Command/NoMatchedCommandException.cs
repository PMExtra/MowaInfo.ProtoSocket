using System;

namespace MowaInfo.ProtoSocket.Command
{
    public class NoMatchedCommandException : Exception
    {
        public NoMatchedCommandException(string typeName) : base($"No matched command of type : {typeName}")
        {
        }
    }
}
