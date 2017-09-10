using System;

namespace MowaInfo.ProtoSocket.Commands
{
    public class NoMatchedCommandException : Exception
    {
        public NoMatchedCommandException(string typeName) : base($"No matched command of type : {typeName}")
        {
        }
    }
}
