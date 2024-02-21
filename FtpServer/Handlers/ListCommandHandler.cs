using FtpServer.Files;
using FtpServer.Connection;
using FtpServer.Handlers.Basics;

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
        IControlConnection connection,
        CancellationToken token = default)
    {
        const string localResponse = "150 Opening data transfer for LIST";
        await connection.SendAsync(localResponse, token);
        
        await connection.DataConnection.OpenAsync(token);
        
        var path = connection.PositionTracker.CurrentPath;
        var items = _filesRepository.GetListByPath(path);
        
        using (connection.DataConnection)
        {
            foreach (var item in items)
            {
                await connection.DataConnection.SendAsync(item, token);
            }
        }
        
        return "226 Transfer complete";
    }
}
