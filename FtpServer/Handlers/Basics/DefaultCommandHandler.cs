using FtpServer.Connection;

namespace FtpServer.Handlers.Basics;

internal interface IFtpCommandHandler
{
    Task<string> HandleAsync(
        FtpCommand command,
        IControlConnection connection,
        CancellationToken token = default);
}

internal class DefaultFtpCommandHandler : IFtpCommandHandler
{
    public Task<string> HandleAsync(
        FtpCommand command,
        IControlConnection connection,
        CancellationToken token = default)
    {
        return Task.FromResult(command.ToResponse());
    }
}
