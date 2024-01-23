using System.IO;
using System.Threading;
using System.Threading.Tasks;
using FtpServer.Connection;

namespace FtpServer.Handlers.Rename;

internal class RenameToCommandHandler : IFtpCommandHandler
{
    public Task<string> HandleAsync(
        FtpCommand command,
        IFtpConnection connection,
        CancellationToken token = default)
    {
        var sourcePath = connection.GetCacheValue(CommandCacheKeys.RenameFrom);
        var destinationPath = connection.PositionTracker.GetPath(command.Value);
        
        if (Directory.Exists(sourcePath))
        {
            Directory.Move(sourcePath, destinationPath);
        }
        else
        {
            File.Move(sourcePath, destinationPath);
        }
        
        return Task.FromResult($"250 File was successfully renamed: {command.Value}.");
    }
}
