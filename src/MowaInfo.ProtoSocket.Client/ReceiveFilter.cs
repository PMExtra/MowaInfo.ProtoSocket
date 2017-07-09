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

            if (!TryRead(stream, out int size))
            {
                return null;
            }

            //if (!Serializer.TryReadLengthPrefix(stream, PrefixStyle.Base128, out int size))
            //{
            //    return null;
            //}

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

        private static bool TryRead(Stream stream, out int size)
        {
            size = 0;
            var byte1 = stream.ReadByte();
            var count = 0;
            while (byte1 != -1)
            {
                size |= (byte1 & 0x7F) << count;
                count += 7;
                if ((byte1 & 0x80) == 0)
                {
                    return true;
                }
                if (stream.Length - stream.Position < 1)
                {
                    return false;
                }
                byte1 = stream.ReadByte();
            }
            return false;
        }
    }
}
