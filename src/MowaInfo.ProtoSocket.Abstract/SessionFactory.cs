﻿using System;
using System.Diagnostics.CodeAnalysis;

namespace MowaInfo.ProtoSocket.Abstract
{
    public abstract class SessionFactory<T> : ISessionFactory
        where T : ISession
    {
        [SuppressMessage("ReSharper", "MemberCanBeProtected.Global")]
        public abstract T CreateSession();

        ISession ISessionFactory.CreateSession()
        {
            return CreateSession();
        }
    }
}
