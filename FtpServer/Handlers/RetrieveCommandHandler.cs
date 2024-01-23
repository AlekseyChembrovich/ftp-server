using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FtpServer.Connection;

namespace FtpServer.Handlers;

internal class RetrieveCommandHandler : IFtpCommandHandler
{
    public async Task<string> HandleAsync(
        FtpCommand command,
        IFtpConnection connection,
        CancellationToken token = default)
    {
        const string localResponse = "150 Opening data transfer for RETR.";
        await connection.SendAsync(localResponse, token);

        await connection.PassiveSession.InitInteractionAsync(token);

        using (connection.PassiveSession)
        {
            var outputStream = connection.PassiveSession.GetStream();

            var filePath = connection.PositionTracker.GetPath(command.Value);
            await using var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);

            await CopyStreamAsync(connection.TransferType, fileStream, outputStream, token);
        }

        return "226 Transfer complete";
    }

    private static Task CopyStreamAsync(
        TransferType transferType,
        Stream input,
        Stream output,
        CancellationToken token = default)
        => transferType switch
        {
            TransferType.Ascii => CopyStreamAsciiAsync(input, output, token),
            TransferType.Binary => CopyStreamBinaryAsync(input, output, token),
            _ => throw new ArgumentException($"Unsupported transfer type: {transferType}.")
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

        await reader.BaseStream.CopyToAsync(writer.BaseStream, token);;
    }
}
