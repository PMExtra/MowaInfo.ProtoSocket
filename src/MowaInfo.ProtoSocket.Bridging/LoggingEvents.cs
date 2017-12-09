using Microsoft.Extensions.Logging;

namespace MowaInfo.ProtoSocket.Bridging
{
    public static class LoggingEvents
    {
        public static readonly EventId InvalidReplyId = new EventId(0, nameof(InvalidReplyId));
    }
}
