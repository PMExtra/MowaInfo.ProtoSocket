using System;
using System.Collections.Generic;
using System.IO;
using MowaInfo.ProtoSocket.Abstract;
using MowaInfo.ProtoSocket.Packing;
using MowaInfo.RedisContext.Core;
using ProtoBuf;
using StackExchange.Redis;

namespace MowaInfo.ProtoSocket.Bridging
{
    public class BridgeObserver<T> : RedisObserver where T : IPackage
    {
        protected Dictionary<Type, List<Delegate>> MessageSubscribers = new Dictionary<Type, List<Delegate>>();
        protected List<Action<T>> Subscribers = new List<Action<T>>();

        public BridgeObserver() : base("")
        {
        }

        public BridgeObserver(string channel) : base(channel)
        {
        }

        protected override void OnNext(RedisChannel channel, RedisValue message)
        {
            OnNext(Deserialize(message));
        }

        protected virtual void OnNext(T package)
        {
            Subscribers.ForEach(action => action(package));
            var message = package.GetMessage();
            if (MessageSubscribers.TryGetValue(message.GetType(), out var list))
            {
                foreach (var action in list)
                {
                    action.DynamicInvoke(message);
                }
            }
        }

        public virtual BridgeObserver<T> Do<TMessage>(Action<TMessage> onNext)
            where TMessage : IMessage
        {
            var type = typeof(TMessage);
            if (!MessageSubscribers.ContainsKey(type))
            {
                MessageSubscribers[type] = new List<Delegate>();
            }

            MessageSubscribers[type].Add(onNext);

            return this;
        }

        private static T Deserialize(byte[] bytes)
        {
            using (var stream = new MemoryStream(bytes))
            {
                return Serializer.Deserialize<T>(stream);
            }
        }
    }
}
