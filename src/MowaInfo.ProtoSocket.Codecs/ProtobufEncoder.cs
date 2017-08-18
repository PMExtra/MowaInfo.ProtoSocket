using System;
using System.Collections.Generic;
using System.IO;
using DotNetty.Codecs;
using DotNetty.Transport.Channels;
using MowaInfo.ProtoSocket.Abstract;
using ProtoBuf;

namespace MowaInfo.ProtoSocket.Codecs
{
    public class ProtobufEncoder<TContainer, TEnum> : MessageToMessageEncoder<TContainer>
        where TEnum : struct, IConvertible
        where TContainer : class, IMessageContainer<TEnum>, new()
    {
        protected override void Encode(IChannelHandlerContext context, TContainer message, List<object> output)
        {
            var buffer = context.Allocator.Buffer();

            byte[] data;
            using (var stream = new MemoryStream())
            {
                Serializer.SerializeWithLengthPrefix(stream, message, PrefixStyle.Base128);
                data = stream.ToArray();
            }
            buffer.WriteBytes(data);
            output.Add(buffer);
        }
    }
}
