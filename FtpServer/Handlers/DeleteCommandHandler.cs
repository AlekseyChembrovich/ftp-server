using System.IO;
using System.Threading;
using System.Threading.Tasks;
using FtpServer.Connection;

namespace FtpServer.Handlers;

internal class DeleteCommandHandler : IFtpCommandHandler
{
    public Task<string> HandleAsync(
        FtpCommand command,
        IFtpConnection connection,
        CancellationToken token = default)
    {
        var path = connection.PositionTracker.GetPath(command.Value);
        
        if (Directory.Exists(path))
        {
            Directory.Delete(path);
        }
        else if (File.Exists(path))
        {
            File.Delete(path);
        }
        else
        {
            return Task.FromResult($"550 File {command.Value} was not found.");
        }
        
        return Task.FromResult($"250 File {command.Value} was successful deleted.");
    }
}
