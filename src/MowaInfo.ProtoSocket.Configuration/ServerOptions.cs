using System.Collections.Generic;

namespace MowaInfo.ProtoSocket.Configuration
{
    public class ServerOptions
    {
        public IEnumerable<string> EndPoints { get; set; }

        public SslOptions Ssl { get; set; }
    }
}
