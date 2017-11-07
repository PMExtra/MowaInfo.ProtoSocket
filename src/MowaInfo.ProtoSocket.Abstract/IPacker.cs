namespace MowaInfo.ProtoSocket.Abstract
{
    // ReSharper disable once TypeParameterCanBeVariant
    public interface IPacker<T>
        where T : IPackage
    {
        T CreatePackage(IMessage message);
        T CreatePackage(int messageType);
    }
}
