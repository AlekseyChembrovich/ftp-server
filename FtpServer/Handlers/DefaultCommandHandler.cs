using System.Threading;
using System.Threading.Tasks;
using FtpServer.Connection;

namespace FtpServer.Handlers;

internal interface IFtpCommandHandler
{
    Task<string> HandleAsync(
        FtpCommand command,
        IFtpConnection connection,
        CancellationToken token = default);
}

internal class DefaultFtpCommandHandler : IFtpCommandHandler
{
    public Task<string> HandleAsync(
        FtpCommand command,
        IFtpConnection connection,
        CancellationToken token = default)
    {
        return Task.FromResult(command.ToResponse());
    }
}
