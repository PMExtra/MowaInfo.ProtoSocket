using System;
using System.Collections.Generic;
using System.IO;
using MowaInfo.ProtoSocket.Server;
using ProtoBuf;
using Xunit;

namespace MowaInfo.ProtoSocket.ServerTests
{
    using ReceiveFilter = ReceiveFilter<RequestInfo<MessageContainer, MessageEnum>, MessageContainer, MessageEnum>;

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

    public class ServerTest
    {
        private readonly MessageContainer _msg = new MessageContainer
        {
            Id = 1,
            ReplyId = 2,
            MessageContent = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaa",
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

        // 组合包
        [Fact]
        public void Combine()
        {
            _bytes = GetBytes(_msg);
            var bytes = new byte[409600];
            var count = _bytes.Length;
            Array.Copy(_bytes, 0, bytes, 405504, count);
            Array.Copy(_bytes, 0, bytes, 405504 + count, count);
            var r = _receiveFilter.Filter(bytes, 405504, 2 * count, true, out _rest);
            Assert.Equal(r.Body, _msg, _compare);
            Assert.NotEqual(_rest, 0);
            r = _receiveFilter.Filter(bytes, 405504 + _rest, _rest, true, out _rest);
            Assert.Equal(r.Body, _msg, _compare);
            Assert.Equal(_rest, 0);
        }

        // 单包
        [Fact]
        public void Single()
        {
            _bytes = GetBytes(_msg);
            var bytes = new byte[409600];
            var count = _bytes.Length;
            Array.Copy(_bytes, 0, bytes, 405504, count);
            var r = _receiveFilter.Filter(bytes, 405504, count, true, out _rest);
            Assert.Equal(r.Body, _msg, _compare);
            Assert.Equal(_rest, 0);
        }

        // 拆包
        [Fact]
        public void Split()
        {
            _bytes = GetBytes(_msg);
            var bytes = new byte[409600];
            var count = _bytes.Length;
            Array.Copy(_bytes, 0, bytes, 405504, count);
            var r = _receiveFilter.Filter(bytes, 405504, 1, true, out _rest);
            Assert.Equal(r, null);
            Assert.Equal(_rest, 0);
            r = _receiveFilter.Filter(bytes, 405504 + 1, count - 1, true, out _rest);
            Assert.Equal(r.Body, _msg, _compare);
            Assert.Equal(_rest, 0);
        }
    }
}
