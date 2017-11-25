using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using MowaInfo.ProtoSocket.Abstract;
using MowaInfo.ProtoSocket.Packing;
using MowaInfo.RedisContext.Core;
using ProtoBuf;
using StackExchange.Redis;

#if NETSTANDARD1_5
using System.Reflection;
#endif

namespace MowaInfo.ProtoSocket.Bridging
{
    public class BridgeObserver<T> : RedisObserver where T : IPackage
    {
        private readonly IServiceProvider _provider;
        protected readonly ICommandResolver Resolver;
        protected Dictionary<Type, List<Delegate>> MessageSubscribers = new Dictionary<Type, List<Delegate>>();
        protected List<Action<T>> Subscribers = new List<Action<T>>();

        public BridgeObserver(IServiceProvider provider) : this(provider, "")
        {
        }

        public BridgeObserver(IServiceProvider provider, string channel) : base(channel)
        {
            _provider = provider;
            Resolver = provider.GetRequiredService(typeof(ICommandResolver<>).MakeGenericType(GetType())) as ICommandResolver;
        }

        protected override void OnNext(RedisChannel channel, RedisValue message)
        {
            OnNext(Deserialize(message));
        }

        protected virtual void OnNext(T package)
        {
            Subscribers.ForEach(action => action(package));
            var message = package.GetMessage();
            var messageType = package.MessageType;
            var commandTypes = Resolver.CommandsOfMessageType(messageType);
            foreach (var commandType in commandTypes)
            {
                var scope = _provider.CreateScope();
                Task.Run(() =>
                {
                    var command = scope.ServiceProvider.GetRequiredService(commandType);
                    var invoker = commandType.GetMethod(nameof(IBridgeCommand<IMessage>.ExecuteAsync));
                    Debug.Assert(invoker != null, nameof(invoker) + " != null");
                    invoker.Invoke(command, new object[] { message });
                }).ContinueWith(_ => scope.Dispose());
            }
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
