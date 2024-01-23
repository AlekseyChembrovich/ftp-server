using System.Threading;
using System.Threading.Tasks;
using FtpServer.Files;
using FtpServer.Connection;

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
        IFtpConnection connection,
        CancellationToken token = default)
    {
        const string localResponse = "150 Opening data transfer for STOR.";
        await connection.SendAsync(localResponse, token);

        await connection.PassiveSession.InitInteractionAsync(token);

        using (connection.PassiveSession)
        {
            var inputStream = connection.PassiveSession.GetStream();
            var filePath = connection.PositionTracker.GetPath(command.Value);
            
            var task = connection.TransferType is TransferType.Binary
                ? _filesRepository.SaveFileBinaryAsync(inputStream, filePath, token)
                : _filesRepository.SaveFileAsciiAsync(inputStream, filePath, token);
            
            await task;
        }
        
        return "226 Transfer complete";
    }
}
