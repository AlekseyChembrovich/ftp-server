using FtpServer.Connection;
using FtpServer.Handlers.Basics;

namespace FtpServer.Handlers;

internal class ChangeDirectoryCommandHandler : IFtpCommandHandler
{
    public Task<string> HandleAsync(
        FtpCommand command,
        IControlConnection connection,
        CancellationToken token = default)
    {
        connection.PositionTracker.Change(command.Value);

        return Task.FromResult($"250 {command.Value} is the current directory.");
    }
}
