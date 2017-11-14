using System;
using System.Linq;
using System.Threading.Tasks;
using Messages;
using Microsoft.Extensions.DependencyInjection;
using MowaInfo.ProtoSocket.Abstract;
using MowaInfo.ProtoSocket.Bridging;
using RedisServer;

namespace Server.command
{
    public class LoginCommand : ICommand<LoginMessage>
    {
        private bool wasExecuted = false;

        public Task ExecuteAsync(ICommandContext context, LoginMessage message)
        {
            var session = context.Session;
            var services = context.RequestServices;
            var redisContext = services.GetService<BridgeRedisContext>();
            var publisher = services.GetService<ApiPublisher>();
            if (message.UserName == null)
            {
                throw new ArgumentNullException(nameof(message.UserName));
            }
            if (!wasExecuted)
            {
                publisher.UserName = message.UserName;
                redisContext.AddPublisher(publisher);
                session.Set(message.UserName, new byte[10]);
                wasExecuted = true;
            }
            publisher.PublishAsync(new SuccessMessage());
            context.Reply(new SuccessMessage());
            return Task.CompletedTask;
        }
    }
}
