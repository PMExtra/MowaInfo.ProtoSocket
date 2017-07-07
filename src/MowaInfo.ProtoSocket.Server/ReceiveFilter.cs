using System;
using System.IO;
using MowaInfo.ProtoSocket.Abstract;
using ProtoBuf;
using SuperSocket.SocketBase.Protocol;

namespace MowaInfo.ProtoSocket.Server
{
    public class ReceiveFilter<TRequstInfo, TContainer, TEnum> : IReceiveFilter<TRequstInfo>
        where TEnum : struct, IConvertible
        where TContainer : class, IMessageContainer<TEnum>, new()
        where TRequstInfo : RequestInfo<TContainer, TEnum>, new()
    {
        protected virtual MemoryStream Stream { get; set; }

        public virtual int LeftBufferSize { get; protected set; }

        public virtual IReceiveFilter<TRequstInfo> NextReceiveFilter => null;

        public virtual FilterState State { get; protected set; }

        public virtual TRequstInfo Filter(byte[] readBuffer, int offset, int length, bool toBeCopied, out int rest)
        {
            rest = 0;

            if (Stream == null)
            {
                Stream = new MemoryStream(length);
            }
            else
            {
                Stream.Position = LeftBufferSize;
            }

            Stream.Write(readBuffer, offset, length);
            Stream.Position = 0;

            if (!Serializer.TryReadLengthPrefix(Stream, PrefixStyle.Base128, out int size))
            {
                LeftBufferSize += length;
                return null;
            }

            var next = Stream.Position + size;
            if (next > Stream.Length)
            {
                LeftBufferSize += length;
                return null;
            }

            TContainer container;

            if (next == Stream.Length)
            {
                container = Serializer.Deserialize<TContainer>(Stream);
            }
            else
            {
                rest = (int)(Stream.Length - Stream.Position - size);

                using (var slice = new MemoryStream(Stream.GetBuffer(), (int)Stream.Position, size))
                {
                    container = Serializer.Deserialize<TContainer>(slice);
                }
            }

            Reset();

            return new TRequstInfo { Body = container };
        }

        public virtual void Reset()
        {
            LeftBufferSize = 0;
            Stream?.Dispose();
            Stream = null;
        }
    }
}
