using System;

namespace MowaInfo.ProtoSocket.Abstract
{
    public interface ISessionFactory
    {
        ISession CreateSession();
    }
}
