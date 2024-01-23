using System.IO;
using System.Threading;
using System.Threading.Tasks;
using FtpServer.Connection;

namespace FtpServer.Handlers;

internal class MakeDirectoryCommandHandler : IFtpCommandHandler
{
    public Task<string> HandleAsync(
        FtpCommand command,
        IFtpConnection connection,
        CancellationToken token = default)
    {
        var path = connection.PositionTracker.GetPath(command.Value);
        
        if (!Directory.Exists(path))
        {
            _ = Directory.CreateDirectory(path);
        }
        else
        {
            return Task.FromResult($"550 Dir {command.Value} already exists.");
        }
        
        return Task.FromResult($"250 Dir {command.Value} was successful created.");
    }
}
