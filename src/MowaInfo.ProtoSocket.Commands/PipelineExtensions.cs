using System;
using DotNetty.Transport.Channels;
using Microsoft.Extensions.DependencyInjection;
using MowaInfo.ProtoSocket.Abstract;

namespace MowaInfo.ProtoSocket.Commands
{
    public static class PipelineExtensions
    {
        public static IChannelPipeline AddRouter<TCommandContext, TPackage>(this IChannelPipeline pipeline, IServiceProvider services)
            where TCommandContext : ICommandContext
            where TPackage : IPackage
        {
            return pipeline.AddLast(services.GetService<CommandRouter<TCommandContext, TPackage>>());
        }
    }
}
