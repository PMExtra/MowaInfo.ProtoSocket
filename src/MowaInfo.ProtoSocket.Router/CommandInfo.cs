using System;
using System.Reflection;
using MowaInfo.ProtoSocket.Abstract;

namespace MowaInfo.ProtoSocket.Router
{
    internal class CommandInfo
    {
        public Type CommandClass { get; set; }

        public ExceptionHandlerAttribute[] ExceptionHandlers { get; set; }

        public CommandFilterAttribute[] Filters { get; set; }

        public MethodInfo Invoker { get; set; }
    }
}
