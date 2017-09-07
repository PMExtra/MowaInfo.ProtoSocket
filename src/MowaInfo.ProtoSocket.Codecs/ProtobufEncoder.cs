using System.Collections.Generic;
using System.IO;
using DotNetty.Codecs;
using DotNetty.Transport.Channels;
using ProtoBuf;

namespace MowaInfo.ProtoSocket.Codecs
{
    public class ProtobufEncoder<TContainer> : MessageToMessageEncoder<TContainer>
    {
        protected override void Encode(IChannelHandlerContext context, TContainer message, List<object> output)
        {
            byte[] data;
            using (var stream = new MemoryStream())
            {
                Serializer.SerializeWithLengthPrefix(stream, message, PrefixStyle.Base128);
                data = stream.ToArray();
            }

            var buffer = context.Allocator.Buffer(data.Length);
            buffer.WriteBytes(data);
            output.Add(buffer);
        }
    }
}
