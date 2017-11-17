using System;
using System.Reflection;
using MowaInfo.ProtoSocket.Commands;

namespace MowaInfo.ProtoSocket.Routing
{
    internal class CommandInfo
    {
        public Type CommandClass { get; set; }

        public ExceptionHandlerAttribute[] ExceptionHandlers { get; set; }

        public CommandFilterAttribute[] Filters { get; set; }

        public MethodInfo Invoker { get; set; }

        public bool IsSynchronized { get; set; }
    }
}
