using System;
using System.Threading.Tasks;
using Messages;
using MowaInfo.ProtoSocket.Abstract;

namespace Server.command
{
    public class LoginCommand : ICommand<LoginMessage>
    {
        public Task ExecuteAsync(ICommandContext context, LoginMessage message)
        {
            var session = context.Session;
            if (message.UserName == null)
            {
                throw new ArgumentNullException(nameof(message.UserName));
            }
            session.Set(message.UserName, new byte[10]);
            context.Reply(new SuccessMessage());
            return Task.CompletedTask;
            //throw new System.NotImplementedException();
        }
    }
}
