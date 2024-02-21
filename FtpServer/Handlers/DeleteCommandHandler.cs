using FtpServer.Connection;
using FtpServer.Handlers.Basics;

namespace FtpServer.Handlers;

internal class DeleteCommandHandler : IFtpCommandHandler
{
    public Task<string> HandleAsync(
        FtpCommand command,
        IControlConnection connection,
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
