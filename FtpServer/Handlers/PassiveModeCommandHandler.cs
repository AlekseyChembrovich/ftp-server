using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using FtpServer.Connection;

namespace FtpServer.Handlers;

internal class PassiveModeCommandHandler : IFtpCommandHandler
{
    public Task<string> HandleAsync(
        FtpCommand command,
        IFtpConnection connection,
        CancellationToken token = default)
    {
        connection.PassiveSession.Start(); // TODO: fix naming
        
        var endPoint = (IPEndPoint)connection.PassiveSession.Listener.LocalEndpoint;
        
        byte[] address = endPoint.Address.GetAddressBytes();
        short portNumber = (short)endPoint.Port;

        byte[] port = BitConverter.GetBytes(portNumber);
        if (BitConverter.IsLittleEndian)
            Array.Reverse(port);

        var localResponse = string.Format(
            "227 Entering Passive Mode ({0},{1},{2},{3},{4},{5})",
            address[0], address[1], address[2], address[3], port[0], port[1]);

        return Task.FromResult(localResponse);
    }
}
