using System;
using System.Collections.Generic;
using MowaInfo.ProtoSocket.Abstract;

namespace MowaInfo.ProtoSocket.Bridging
{
    internal class CommandResolver<TObserver> : ICommandResolver<TObserver>
    {
        private readonly ICommandResolver _resolver;

        public CommandResolver(CommandResolverFactory factory)
        {
            _resolver = factory.CreateResolver(typeof(TObserver));
        }

        public IEnumerable<Type> CommandsOfMessageType(int messageType)
        {
            return _resolver.CommandsOfMessageType(messageType);
        }
    }

    internal class CommandResolver : ICommandResolver
    {
        private readonly IReadOnlyDictionary<int, Type[]> _commandsByMessageType;

        public CommandResolver(IReadOnlyDictionary<int, Type[]> commandsByMessageType)
        {
            _commandsByMessageType = commandsByMessageType;
        }

        public IEnumerable<Type> CommandsOfMessageType(int messageType)
        {
            return _commandsByMessageType[messageType];
        }
    }
}
