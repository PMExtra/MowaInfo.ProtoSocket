using System;

namespace MowaInfo.ProtoSocket.Abstract
{
    public interface IMessageContainer<TEnum> where TEnum : struct, IConvertible
    {
        uint Id { get; set; }

        uint? ReplyId { get; set; }

        TEnum Type { get; set; }
    }
}
