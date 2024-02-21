using FtpServer.Connection;
using FtpServer.Handlers.Basics;

namespace FtpServer.Handlers.Rename;

internal class RenameToCommandHandler : IFtpCommandHandler
{
    public Task<string> HandleAsync(
        FtpCommand command,
        IControlConnection connection,
        CancellationToken token = default)
    {
        var sourcePath = connection.ConnectionBuffer.GetCacheValue(CommandCacheKeys.RenameFrom);
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
