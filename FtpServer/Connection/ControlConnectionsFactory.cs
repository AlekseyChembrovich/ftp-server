using FtpServer.Files;
using System.Net.Sockets;

namespace FtpServer.Connection;

internal interface IControlConnectionsFactory
{
    IControlConnection Create(TcpClient client);
}

internal class ControlConnectionsFactory : IControlConnectionsFactory
{
    public IControlConnection Create(TcpClient client) =>
        new ControlConnection(
            client,
            new CommandHandlersFactory(),
            new DirectoryPositionTracker(),
            new ControlConnectionBuffer()
        );
}
