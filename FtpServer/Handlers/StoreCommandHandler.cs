using FtpServer.Files;
using FtpServer.Connection;
using FtpServer.Handlers.Basics;
using FtpServer.Handlers.Type;

namespace FtpServer.Handlers;

internal class StoreCommandHandler : IFtpCommandHandler
{
    private readonly IFilesRepository _filesRepository;
    
    public StoreCommandHandler(IFilesRepository filesRepository)
    {
        _filesRepository = filesRepository;
    }
    
    public async Task<string> HandleAsync(
        FtpCommand command,
        IControlConnection connection,
        CancellationToken token = default)
    {
        const string localResponse = "150 Opening data transfer for STOR.";
        await connection.SendAsync(localResponse, token);
        
        await connection.DataConnection.OpenAsync(token);
        
        using (connection.DataConnection)
        {
            var inputStream = connection.DataConnection.GetStream();
            var filePath = connection.PositionTracker.GetPath(command.Value);
            
            var task = connection.CodingType is CodingType.Binary
                ? _filesRepository.SaveFileBinaryAsync(inputStream, filePath, token)
                : _filesRepository.SaveFileAsciiAsync(inputStream, filePath, token);
            
            await task;
        }
        
        return "226 Transfer complete";
    }
}
