using System;
using System.Collections.Generic;
using System.IO;
using ProtoBuf;
using SuperSocket.ProtoBase;
using Xunit;

namespace MowaInfo.ProtoSocket.Client.Tests
{
    using ReceiveFilter = ReceiveFilter<PackageInfo<MessageContainer, MessageEnum>, MessageContainer, MessageEnum>;

    public class Compare : IEqualityComparer<MessageContainer>
    {
        public bool Equals(MessageContainer x, MessageContainer y)
        {
            return x.Type == y.Type && x.Id == y.Id && x.ReplyId == y.ReplyId && y.MessageContent == x.MessageContent;
        }

        public int GetHashCode(MessageContainer obj)
        {
            throw new NotImplementedException();
        }
    }

    public class ClientTest
    {
        private readonly MessageContainer _msg = new MessageContainer
        {
            Id = 1,
            ReplyId = 2,
            MessageContent =
                "aassssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssa",
            Type = MessageEnum.Success
        };

        private readonly Compare _compare = new Compare();
        private readonly ReceiveFilter _receiveFilter = new ReceiveFilter();
        private int _rest;
        private byte[] _bytes;

        private static byte[] GetBytes(MessageContainer msg)
        {
            using (var stream = new MemoryStream())
            {
                Serializer.SerializeWithLengthPrefix(stream, msg, PrefixStyle.Base128);
                return stream.ToArray();
            }
        }

        [Fact]
        public void Combine()
        {
            _bytes = GetBytes(_msg);
            var byte1 = new byte[2 * _bytes.Length];
            _bytes.CopyTo(byte1, 0);
            _bytes.CopyTo(byte1, _bytes.Length);
            var y = new ArraySegment<byte>(byte1);
            var x = new BufferList { y };
            var count = x.Total;
            var r = _receiveFilter.Filter(x, out _rest);
            Assert.Equal(r.Body, _msg, _compare);
            Assert.NotEqual(_rest, count);
            var a = new ArraySegment<byte>(y.Array, y.Offset + y.Count - _rest, _rest);
            var z = new BufferList { a };
            r = _receiveFilter.Filter(z, out _rest);
            Assert.Equal(r.Body, _msg, _compare);
            Assert.Equal(_rest, 0);
        }

        [Fact]
        public void Single()
        {
            _bytes = GetBytes(_msg);
            var y = new ArraySegment<byte>(_bytes);
            var x = new BufferList { y };
            var r = _receiveFilter.Filter(x, out _rest);
            Assert.Equal(r.Body, _msg, _compare);
            Assert.Equal(_rest, 0);
        }

        [Fact]
        public void Split()
        {
            _bytes = GetBytes(_msg);
            var bytes = new byte[4096];
            bytes[0] = _bytes[0];
            var x = new BufferList { new ArraySegment<byte>(bytes, 0, 1) };
            var count = _bytes.Length;
            var r = _receiveFilter.Filter(x, out _rest);
            Assert.Equal(r, null);
            Assert.Equal(_rest, 0);
            var a = new byte[count - 1];
            Array.Copy(_bytes, 1, a, 0, count - 1);
            x.Add(new ArraySegment<byte>(a));
            r = _receiveFilter.Filter(x, out _rest);
            Assert.Equal(r.Body, _msg, _compare);
            Assert.Equal(_rest, 0);
        }
    }
}
