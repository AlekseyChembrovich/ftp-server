using System.Threading;
using System.Threading.Tasks;
using FtpServer.Connection;

namespace FtpServer.Handlers;

internal class TypeCommandHandler : IFtpCommandHandler
{
    public Task<string> HandleAsync(
        FtpCommand command,
        IFtpConnection connection,
        CancellationToken token = default)
    {
        connection.TransferType = command.Value.ParseTransferType();

        return Task.FromResult($"200 Type {command.Value} is set.");
    }
}
