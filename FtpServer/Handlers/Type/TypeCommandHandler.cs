using FtpServer.Connection;
using FtpServer.Handlers.Basics;

namespace FtpServer.Handlers.Type;

internal class TypeCommandHandler : IFtpCommandHandler
{
    public Task<string> HandleAsync(
        FtpCommand command,
        IControlConnection connection,
        CancellationToken token = default)
    {
        var codingType = command.Value.ParseCodingType();
        connection.SetupCodingType(codingType);
        
        return Task.FromResult($"200 Type {command.Value} is set.");
    }
}
