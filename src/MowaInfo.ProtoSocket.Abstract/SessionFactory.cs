using System.Diagnostics.CodeAnalysis;

namespace MowaInfo.ProtoSocket.Abstract
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
