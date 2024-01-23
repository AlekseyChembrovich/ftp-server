using System.Threading;
using System.Threading.Tasks;
using FtpServer.Connection;

namespace FtpServer.Handlers;

internal class ChangeDirectoryCommandHandler : IFtpCommandHandler
{
    public Task<string> HandleAsync(
        FtpCommand command,
        IFtpConnection connection,
        CancellationToken token = default)
    {
        connection.PositionTracker.Change(command.Value);

        return Task.FromResult($"250 {command.Value} is the current directory.");
    }
}
