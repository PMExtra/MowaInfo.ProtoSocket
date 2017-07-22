using System;
using System.IO;
using MowaInfo.ProtoSocket.Abstract;
using ProtoBuf;
using SuperSocket.ProtoBase;

namespace MowaInfo.ProtoSocket.Client
{
    public class ReceiveFilter<TPackageInfo, TContainer, TEnum> : IReceiveFilter<TPackageInfo>
        where TEnum : struct, IConvertible
        where TContainer : class, IMessageContainer<TEnum>, new()
        where TPackageInfo : PackageInfo<TContainer, TEnum>, new()
    {
        public virtual IReceiveFilter<TPackageInfo> NextReceiveFilter => null;

        public virtual FilterState State { get; protected set; }

        public virtual TPackageInfo Filter(BufferList data, out int rest)
        {
            rest = 0;

            var stream = this.GetBufferStream(data);

            int size;
            try
            {
                if (!Serializer.TryReadLengthPrefix(stream, PrefixStyle.Base128, out size))
                {
                    return null;
                }
            }
            catch (EndOfStreamException)
            {
                return null;
            }

            var end = stream.Position + size;
            if (end > stream.Length)
            {
                return null;
            }

            TContainer container;
            rest = (int)(stream.Length - stream.Position - size);
            var buffer = new byte[size];
            stream.Read(buffer, 0, size);
            using (var slice = new MemoryStream(buffer))
            {
                container = Serializer.Deserialize<TContainer>(slice);
            }

            return new TPackageInfo { Body = container };
        }

        public virtual void Reset()
        {
        }
    }
}
