using System.Net;
using FtpServer.Connection;
using FtpServer.Connection.Data;
using FtpServer.Handlers.Basics;

namespace FtpServer.Handlers.Mode;

internal class ActiveModeCommandHandler : IFtpCommandHandler
{
    public Task<string> HandleAsync(
        FtpCommand command,
        IControlConnection connection,
        CancellationToken token = default)
    {
        var parts = command.Value.Split(','); // PORT 127,0,0,1,221,212
        var ipParts = parts.Take(4).Select(int.Parse).ToArray();
        var portParts = parts.Skip(4).Select(byte.Parse).ToArray();
        
        var ipAddress = IPAddress.Parse(string.Join('.', ipParts));
        var port = portParts[0] * 256 + portParts[1];
        
        connection.SetupDataConnection(DataConnectionMode.Active, ipAddress, port);
        // await connection.DataConnection.StartAsync(token);
        
        return Task.FromResult("200 Entering Active Mode");
    }
}
