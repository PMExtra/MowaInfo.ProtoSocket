using DotNetty.Buffers;
using DotNetty.Transport.Channels.Embedded;
using MowaInfo.ProtoSocket.Abstract;
using MowaInfo.ProtoSocket.Codecs.Tests.Models;
using MowaInfo.ProtoSocket.Packing;
using Xunit;

namespace MowaInfo.ProtoSocket.Codecs.Tests
{
    public class ProtobufCodecTest
    {
        private readonly IPacker<MessagePackage> _packer = new Packer<MessagePackage>();

        [Theory]
        [InlineData("Hello world!")]
        public void NormalTest(string testContent)
        {
            var channel = new EmbeddedChannel(new ProtobufEncoder<MessagePackage>(), new ProtobufDecoder<MessagePackage>());

            var content = _packer.CreatePackage(new TestMessage { TestContent = testContent });
            Assert.True(channel.WriteOutbound(content));

            var buffer = channel.ReadOutbound<IByteBuffer>();
            Assert.NotNull(buffer);
            Assert.True(buffer.ReadableBytes > 0);

            var data = new byte[buffer.ReadableBytes];
            buffer.ReadBytes(data);

            var inputBuffer = Unpooled.WrappedBuffer(data);
            Assert.True(channel.WriteInbound(inputBuffer));

            var container = channel.ReadInbound<MessagePackage>();
            Assert.NotNull(container);
            Assert.Equal(MessageType.Test, container.MessageType);
            Assert.Equal(testContent, container.Test.TestContent);

            Assert.False(channel.Finish());
        }

        [Theory]
        [InlineData("Hello world!")]
        public void SplitedTest(string testContent)
        {
            var channel = new EmbeddedChannel(new ProtobufEncoder<MessagePackage>(), new ProtobufDecoder<MessagePackage>());

            var content = _packer.CreatePackage(new TestMessage { TestContent = testContent });
            Assert.True(channel.WriteOutbound(content));

            var buffer = channel.ReadOutbound<IByteBuffer>();
            Assert.NotNull(buffer);
            Assert.True(buffer.ReadableBytes > 0);

            var data = new byte[buffer.ReadableBytes];
            buffer.ReadBytes(data);

            var inputBuffer1 = Unpooled.WrappedBuffer(data, 0, data.Length / 2);
            var inputBuffer2 = Unpooled.WrappedBuffer(data, data.Length / 2, data.Length - data.Length / 2);
            Assert.False(channel.WriteInbound(inputBuffer1));
            Assert.True(channel.WriteInbound(inputBuffer2));

            var container = channel.ReadInbound<MessagePackage>();
            Assert.NotNull(container);
            Assert.Equal(MessageType.Test, container.MessageType);
            Assert.Equal(testContent, container.Test.TestContent);

            Assert.False(channel.Finish());
        }

        [Theory]
        [InlineData("Hello world!")]
        public void CombinedTest(string testContent)
        {
            var channel = new EmbeddedChannel(new ProtobufEncoder<MessagePackage>(), new ProtobufDecoder<MessagePackage>());

            var content1 = _packer.CreatePackage(new TestMessage { TestContent = testContent });
            content1.Id = 1;
            var content2 = _packer.CreatePackage(new TestMessage { TestContent = testContent });
            content2.Id = 2;
            Assert.True(channel.WriteOutbound(content1));
            Assert.True(channel.WriteOutbound(content2));

            var buffer1 = channel.ReadOutbound<IByteBuffer>();
            var buffer2 = channel.ReadOutbound<IByteBuffer>();
            Assert.NotNull(buffer1);
            Assert.True(buffer1.ReadableBytes > 0);
            Assert.NotNull(buffer2);
            Assert.True(buffer2.ReadableBytes > 0);

            var length1 = buffer1.ReadableBytes;
            var length2 = buffer2.ReadableBytes;
            var data = new byte[length1 + length2];
            buffer1.ReadBytes(data, 0, length1);
            buffer2.ReadBytes(data, length1, length2);

            var inputBuffer = Unpooled.WrappedBuffer(data);
            Assert.True(channel.WriteInbound(inputBuffer));

            var container1 = channel.ReadInbound<MessagePackage>();
            var container2 = channel.ReadInbound<MessagePackage>();
            Assert.NotNull(container1);
            Assert.NotNull(container2);
            Assert.True(container1.Id == 1);
            Assert.Equal(MessageType.Test, container1.MessageType);
            Assert.Equal(testContent, container1.Test.TestContent);
            Assert.True(container2.Id == 2);
            Assert.Equal(MessageType.Test, container2.MessageType);
            Assert.Equal(testContent, container2.Test.TestContent);

            Assert.False(channel.Finish());
        }

        [Fact]
        public void EmptyTest()
        {
            var channel = new EmbeddedChannel(new ProtobufEncoder<MessagePackage>(), new ProtobufDecoder<MessagePackage>());

            var container = channel.ReadInbound<MessagePackage>();
            Assert.Null(container);

            Assert.False(channel.Finish());
        }
    }
}
