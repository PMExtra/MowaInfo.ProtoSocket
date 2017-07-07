using System;
using System.Diagnostics.CodeAnalysis;
using MowaInfo.ProtoSocket.Abstract;
using SuperSocket.SocketBase.Protocol;

namespace MowaInfo.ProtoSocket.Server
{
    public class RequestInfo<TContainer, TEnum> : IRequestInfo
        where TEnum : struct, IConvertible
        where TContainer : class, IMessageContainer<TEnum>
    {
        public virtual TContainer Body { get; set; }

        [SuppressMessage("ReSharper", "SpecifyACultureInStringConversionExplicitly")]
        public virtual string Key => Body.Type.ToString();

        public static implicit operator TContainer(RequestInfo<TContainer, TEnum> requestInfo)
        {
            return requestInfo.Body;
        }
    }
}
