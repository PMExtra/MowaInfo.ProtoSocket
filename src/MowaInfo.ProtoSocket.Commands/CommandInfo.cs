using System;
using System.Reflection;
using MowaInfo.ProtoSocket.Abstract;
using MowaInfo.ProtoSocket.Annotations;

namespace MowaInfo.ProtoSocket.Commands
{
    internal class CommandInfo
    {
        public Type CommandClass { get; set; }

        public ExceptionHandlerAttribute[] ExceptionHandlers { get; set; }

        public CommandFilterAttribute[] Filters { get; set; }

        public MethodInfo Invoker { get; set; }

        public bool Synchronized { get; set; }
    }
}
