using FtpServer.Files;
using System.Net.Sockets;
using FtpServer.Handlers;

namespace FtpServer.Connection;

internal interface IConnectionsFactory
{
    IFtpConnection Create(NetworkStream networkStream);
}

internal class FtpConnectionsFactory : IConnectionsFactory
{
    public IFtpConnection Create(NetworkStream networkStream) =>
        new FtpConnection(
            networkStream,
            new CommandHandlersFactory(),
            new PassiveSession(),
            new PositionTracker());
}
