using System;
using System.Collections.Generic;
using System.IO;
using MowaInfo.ProtoSocket.Client;
using ProtoBuf;
using SuperSocket.ProtoBase;
using Xunit;

namespace MowaInfo.ProtoSocket.ClientTests
{
    using ReceiveFilter = ReceiveFilter<Client.PackageInfo<MessageContainer, MessageEnum>, MessageContainer, MessageEnum>;

    public class Compare : IEqualityComparer<MessageContainer>
    {
        public bool Equals(MessageContainer x, MessageContainer y)
        {
            //!((x.ReplyId == null && y.ReplyId != null) || (y.ReplyId == null && x.ReplyId != null) || x.ReplyId != y.ReplyId)
            if (x.Type == y.Type && x.Id == y.Id && x.ReplyId == y.ReplyId && y.MessageContent == x.MessageContent)
            {
                return true;
            }
            return false;
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
            MessageContent = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaa",
            Type = MessageEnum.Success
        };

        private readonly Compare _compare = new Compare();
        private readonly ReceiveFilter _receiveFilter = new ReceiveFilter();
        private int _rest;
        private byte[] _bytes;

        private byte[] GetBytes(MessageContainer msg)
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
            var y = new ArraySegment<byte>(_bytes);
            var x = new BufferList { y, y };
            var count = x.Total;
            var r = _receiveFilter.Filter(x, out _rest);
            Assert.Equal(r.Body, _msg, _compare);
            Assert.NotEqual(_rest, count);
            var z = new BufferList { x.Last };
            r = _receiveFilter.Filter(z, out _rest);
            Assert.Equal(r.Body, _msg, _compare);
            Assert.Equal(_rest, 0);
        }

        // 单个包
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

        //分组
        [Fact]
        public void Split()
        {
            _bytes = GetBytes(_msg);
            var y = new ArraySegment<byte>(new[] { _bytes[0] });
            var x = new BufferList { y };
            var count = _bytes.Length;
            var r = _receiveFilter.Filter(x, out _rest);
            Assert.Equal(r, null);
            Assert.Equal(_rest, 0);
            var a = new byte[38];
            Array.Copy(_bytes, 1, a, 0, count - 1);
            x.Add(new ArraySegment<byte>(a));
            r = _receiveFilter.Filter(x, out _rest);
            Assert.Equal(r.Body, _msg, _compare);
            Assert.Equal(_rest, 0);
        }
    }
}
