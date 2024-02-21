using System.Text;
using FtpServer.Connection;
using FtpServer.Handlers.Basics;
using FtpServer.Handlers.Type;

namespace FtpServer.Handlers;

internal class RetrieveCommandHandler : IFtpCommandHandler
{
    public async Task<string> HandleAsync(
        FtpCommand command,
        IControlConnection connection,
        CancellationToken token = default)
    {
        const string localResponse = "150 Opening data transfer for RETR.";
        await connection.SendAsync(localResponse, token);
        
        await connection.DataConnection.OpenAsync(token);
        
        using (connection.DataConnection)
        {
            var outputStream = connection.DataConnection.GetStream();
            
            var filePath = connection.PositionTracker.GetPath(command.Value);
            await using var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            
            await CopyStreamAsync(connection.CodingType, fileStream, outputStream, token);
        }
        
        return "226 Transfer complete";
    }
    
    private static Task CopyStreamAsync(
        CodingType codingType,
        Stream input,
        Stream output,
        CancellationToken token = default)
        => codingType switch
        {
            CodingType.Ascii => CopyStreamAsciiAsync(input, output, token),
            CodingType.Binary => CopyStreamBinaryAsync(input, output, token),
            _ => throw new ArgumentException($"Unsupported coding type: {codingType}.")
        };
    
    private static Task CopyStreamBinaryAsync(
        Stream input,
        Stream output,
        CancellationToken token = default)
        => input.CopyToAsync(output, token);
    
    private static async Task CopyStreamAsciiAsync(
        Stream input,
        Stream output,
        CancellationToken token = default)
    {
        using var reader = new StreamReader(input, Encoding.ASCII);
        await using var writer = new StreamWriter(output, Encoding.ASCII);
        
        await reader.BaseStream.CopyToAsync(writer.BaseStream, token);
    }
}
