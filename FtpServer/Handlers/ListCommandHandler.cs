using FtpServer.Files;
using FtpServer.Connection;

namespace FtpServer.Handlers;

internal class ListCommandHandler : IFtpCommandHandler
{
    private readonly IFilesRepository _filesRepository;

    public ListCommandHandler(IFilesRepository filesRepository)
    {
        _filesRepository = filesRepository;
    }
    
    public async Task<string> HandleAsync(
        FtpCommand command,
        IFtpConnection connection,
        CancellationToken token = default)
    {
        const string localResponse = "150 Opening data transfer for LIST";
        await connection.SendAsync(localResponse, token);
        
        await connection.PassiveSession.InitInteractionAsync(token);

        var path = connection.PositionTracker.CurrentPath;
        var items = _filesRepository.GetListByPath(path);

        using (connection.PassiveSession)
        {
            foreach (var item in items)
            {
                await connection.PassiveSession.SendAsync(item, token);
            }   
        }

        return "226 Transfer complete";
    }
}
