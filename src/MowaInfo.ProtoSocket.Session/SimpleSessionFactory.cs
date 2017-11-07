using MowaInfo.ProtoSocket.Abstract;

namespace MowaInfo.ProtoSocket.Session
{
    public class SimpleSessionFactory<T> : SessionFactory<T>
        where T : ISession, new()
    {
        public override T CreateSession()
        {
            return new T();
        }
    }
}
