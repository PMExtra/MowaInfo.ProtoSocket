using System.Security.Authentication;

namespace MowaInfo.ProtoSocket.Configuration
{
    public class SslOptions
    {
        public SslProtocols Protocol { get; set; }

        public bool NegotiateClientCertificate { get; set; }

        public bool CheckCertificateRevocation { get; set; }

        public CertificateOptions Certificate { get; set; }
    }
}
