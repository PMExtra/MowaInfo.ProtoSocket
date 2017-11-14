﻿using System.IO;
using System.Threading.Tasks;
using Generator;
using MowaInfo.ProtoSocket.Abstract;
using MowaInfo.RedisContext.Core;
using ProtoBuf;

namespace MowaInfo.ProtoSocket.Bridging
{
    public class Publisher<T> : RedisPublisher where T : IPackage
    {
        protected readonly IPackageNumberer PackageNumberer;
        protected readonly IPacker<T> Packer;

        public Publisher(IPacker<T> packer, IPackageNumberer packageNumberer) : base("")
        {
            Packer = packer;
            PackageNumberer = packageNumberer;
        }

        public virtual long Publish(IMessage message)
        {
            return Publish(Packer.CreatePackage(message));
        }

        protected virtual long Publish(T package)
        {
            package.Id = PackageNumberer.NextId();
            return base.Publish(Serialize(package));
        }

        public virtual Task<long> PublishAsync(IMessage message)
        {
            return PublishAsync(Packer.CreatePackage(message));
        }

        protected virtual Task<long> PublishAsync(T package)
        {
            package.Id = PackageNumberer.NextId();
            return base.PublishAsync(Serialize(package));
        }

        private static byte[] Serialize(T instance)
        {
            using (var stream = new MemoryStream())
            {
                Serializer.Serialize(stream, instance);
                return stream.ToArray();
            }
        }
    }
}
