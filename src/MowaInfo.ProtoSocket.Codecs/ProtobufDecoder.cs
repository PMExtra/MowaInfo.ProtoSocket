using System;
using System.Collections.Generic;
using System.IO;
using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Transport.Channels;
using ProtoBuf;

namespace MowaInfo.ProtoSocket.Codecs
{
    public class ProtobufDecoder<TContainer> : ByteToMessageDecoder
        where TContainer : new()
    {
        protected override void Decode(IChannelHandlerContext context, IByteBuffer input, List<object> output)
        {
            if (!input.IsReadable())
            {
                return;
            }

            var inputStream = new ReadOnlyByteBufferStream(input, false);
            input.MarkReaderIndex();
            try
            {
                var container = Serializer.DeserializeWithLengthPrefix<TContainer>(inputStream, PrefixStyle.Base128);
                output.Add(container);
            }
            catch (EndOfStreamException)
            {
                input.ResetReaderIndex();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
                throw new CodecException(exception);
            }
            finally
            {
                inputStream.Dispose();
            }
        }
    }
}
