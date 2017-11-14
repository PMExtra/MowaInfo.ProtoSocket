using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using DotNetty.Transport.Bootstrapping;
using MowaInfo.ProtoSocket.Abstract;

namespace MowaInfo.ProtoSocket
{
    internal class TcpServer : IServer
    {
        private readonly ServerBootstrap _bootstrap;
        private readonly IEnumerable<IPEndPoint> _endPoints;

        public TcpServer(ServerBootstrap bootstrap, IEnumerable<IPEndPoint> endPoints)
        {
            _bootstrap = bootstrap;
            _endPoints = endPoints;
        }

        public async Task StartAsync()
        {
            foreach (var endPoint in _endPoints)
            {
                await _bootstrap.BindAsync(endPoint);
            }
        }

        public Task StopAsync()
        {
            throw new NotImplementedException();
        }
    }
}
