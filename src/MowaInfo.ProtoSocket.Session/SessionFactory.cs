using System.Diagnostics.CodeAnalysis;
using MowaInfo.ProtoSocket.Abstract;

namespace MowaInfo.ProtoSocket.Session
{
    public abstract class SessionFactory<T> : ISessionFactory
        where T : ISession
    {
        ISession ISessionFactory.CreateSession()
        {
            return CreateSession();
        }

        [SuppressMessage("ReSharper", "MemberCanBeProtected.Global")]
        public abstract T CreateSession();
    }
}
