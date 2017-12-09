using ATest;
using ProtoBuf;

namespace AutoGenerateTest
{
    [ProtoContract]
    public class ComparePackage : Package
    {
        [ProtoMember(5)]
        public WelcomeMessage WelcomeMessage { get; set; }
    }
}
