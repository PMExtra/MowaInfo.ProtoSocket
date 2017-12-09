namespace MowaInfo.ProtoSocket.Abstract
{
    public interface IPacker
    {


        IPackage CreatePackage(IDownMessage message);
    }
}
