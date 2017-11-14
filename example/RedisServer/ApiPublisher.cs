using Generator;
using Messages;
using MowaInfo.ProtoSocket.Abstract;
using MowaInfo.ProtoSocket.Bridging;

namespace RedisServer
{
    public class ApiPublisher : Publisher<Package>
    {
        private string userName;

        public ApiPublisher(IPacker<Package> packer, IPackageNumberer packageNumberer) : base(packer, packageNumberer)
        {
        }

        public string UserName
        {
            get => userName;
            set
            {
                userName = value;
                Channel = $"Client:{userName}";
            }
        }
    }
}
