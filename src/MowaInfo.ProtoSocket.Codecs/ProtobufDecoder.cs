using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Transport.Channels;
using MowaInfo.ProtoSocket.Abstract;
using ProtoBuf;

namespace MowaInfo.ProtoSocket.Codecs
{
    public class ProtobufDecoder<TContainer, TEnum> : ByteToMessageDecoder
        where TEnum : struct, IConvertible
        where TContainer : class, IMessageContainer<TEnum>, new()
    {
        protected override void Decode(IChannelHandlerContext context, IByteBuffer input, List<object> output)
        {
            var length = input.ReadableBytes;
            if (length <= 0)
            {
                return;
            }

            Stream inputStream = null;
            try
            {
                if (input.IoBufferCount == 1)
                {
                    var bytes = input.GetIoBuffer(input.ReaderIndex, length);
                    inputStream = new MemoryStream(bytes.ToArray());
                }
                else
                {
                    inputStream = new ReadOnlyByteBufferStream(input, false);
                }

                {
                    int size;
                    try
                    {
                        if (!Serializer.TryReadLengthPrefix(inputStream, PrefixStyle.Base128, out size))
                        {
                            return;
                        }
                    }
                    catch (EndOfStreamException)
                    {
                        return;
                    }

                    if (size > length)
                    {
                        return;
                    }
                    var buffer = new byte[size];
                    TContainer container;
                    inputStream.Read(buffer, 0, size);
                    var x = new byte[inputStream.Position];
                    input.ReadBytes(x);
                    using (var slice = new MemoryStream(buffer))
                    {
                        container = Serializer.Deserialize<TContainer>(slice);
                    }

                    output.Add(container);
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
                throw new CodecException(exception);
            }
            finally
            {
                inputStream?.Dispose();
            }
        }
    }
}
