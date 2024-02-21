using FtpServer.Connection;
using FtpServer.Handlers.Basics;

namespace FtpServer.Handlers.Rename;

internal class RenameFromCommandHandler : IFtpCommandHandler
{
    public Task<string> HandleAsync(
        FtpCommand command,
        IControlConnection connection,
        CancellationToken token = default)
    {
        var path = connection.PositionTracker.GetPath(command.Value);
        
        var message = default(string);
        if (Directory.Exists(path))
        {
            message = $"350 Directory ready to be renamed: {command.Value}.";
            
            connection.ConnectionBuffer.SetCacheValue(CommandCacheKeys.RenameFrom, path);
        }
        else if (File.Exists(path))
        {
            message = $"350 File ready to be renamed: {command.Value}.";
            
            connection.ConnectionBuffer.SetCacheValue(CommandCacheKeys.RenameFrom, path);
        }
        else
        {
            message = $"550 Cannot rename file: {command.Value}.";
        }
        
        return Task.FromResult(message);
    }
}
