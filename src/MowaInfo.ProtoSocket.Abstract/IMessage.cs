using System;
using System.Diagnostics.CodeAnalysis;

namespace MowaInfo.ProtoSocket.Abstract
{
    [SuppressMessage("ReSharper", "TypeParameterCanBeVariant")]
    public interface IMessage<TEnum> where TEnum : struct, IConvertible
    {
        TEnum MessageType { get; }
    }
}
