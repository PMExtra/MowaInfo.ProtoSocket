using System;
using System.Collections.Generic;
using System.IO;
using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Transport.Channels;
using ProtoBuf;

namespace MowaInfo.ProtoSocket.Codecs
{
    public class ProtobufDecoder<TPackage> : ByteToMessageDecoder
        where TPackage : new()
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
                var package = Serializer.DeserializeWithLengthPrefix<TPackage>(inputStream, PrefixStyle.Base128);
                output.Add(package);
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
