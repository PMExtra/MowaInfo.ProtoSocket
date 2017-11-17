using MowaInfo.ProtoSocket.Abstract;

namespace MowaInfo.ProtoSocket.Session
{
    public interface ISessionFactory
    {
        ISession CreateSession();
    }
}
