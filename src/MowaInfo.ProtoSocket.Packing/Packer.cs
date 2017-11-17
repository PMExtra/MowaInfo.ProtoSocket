using MowaInfo.ProtoSocket.Abstract;

namespace MowaInfo.ProtoSocket.Packing
{
    public class Packer<T> : IPacker<T>
        where T : IPackage, new()
    {
        public T CreatePackage(IMessage message)
        {
            var package = new T();
            package.SetMessage(message);
            return package;
        }
    }
}
