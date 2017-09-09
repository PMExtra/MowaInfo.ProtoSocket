﻿using System;

namespace MowaInfo.ProtoSocket
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public abstract class CommandFilterAttribute : Attribute
    {
        public int Order { get; set; }

        public abstract void OnCommandExecuting();

        public abstract void OnCommandExecuted();
    }
}