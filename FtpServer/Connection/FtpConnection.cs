using System.Text;
using FtpServer.Files;
using System.Net.Sockets;

namespace FtpServer.Connection;

internal interface IFtpConnection : IDisposable
{
    IPassiveSession PassiveSession { get; }

    IPositionTracker PositionTracker { get; }

    TransferType TransferType { get; set; }
    
    Task BeginInteractionAsync(CancellationToken token = default);
    
    Task SendAsync(string response, CancellationToken token = default);

    void SetCacheValue(string key, string value);
    
    string GetCacheValue(string key);
}

internal class FtpConnection : IFtpConnection
{
    private readonly NetworkStream _networkStream;
    private readonly StreamWriter _writer;
    private readonly StreamReader _reader;
    private readonly ICommandHandlersFactory _handlersFactory;

    public IPassiveSession PassiveSession { get; }
    public IPositionTracker PositionTracker { get; }

    public TransferType TransferType { get; set; }
    
    private readonly IDictionary<string, string> _cache = new Dictionary<string, string>();
    
    public FtpConnection(
        NetworkStream networkStream,
        ICommandHandlersFactory handlersFactory,
        IPassiveSession passiveSession,
        IPositionTracker positionTracker)
    {
        _networkStream = networkStream;
        _writer = new StreamWriter(_networkStream, Encoding.ASCII);
        _reader = new StreamReader(_networkStream, Encoding.ASCII);
        _handlersFactory = handlersFactory;
        
        PassiveSession = passiveSession;
        PositionTracker = positionTracker;
    }
    
    public async Task BeginInteractionAsync(CancellationToken token = default)
    {
        try
        {
            await SendAsync("220 Welcome to the FTP Server.", token);
            
            string? line;
            while (!string.IsNullOrWhiteSpace(line = await _reader.ReadLineAsync(token)))
            {
                Console.WriteLine(line);

                var ftpCommand = line.ParseFtpCommand();

                var handler = _handlersFactory.Create(ftpCommand.Type);

                var response = await handler!.HandleAsync(ftpCommand, this, token);

                await SendAsync(response, token);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[{ex.GetType()}] at Ftp connection level." +
                              $" An error occurred with message: {ex.Message}" +
                              $" and inner message: {ex.InnerException?.Message}");
            
            throw;
        }
    }
    
    public async Task SendAsync(string response, CancellationToken token = default)
    {
        await _writer.WriteLineAsync(response);
        await _writer.FlushAsync(token);
    }

    public void SetCacheValue(string key, string value)
    {
        _cache[key] = value;
    }

    public string GetCacheValue(string key)
    {
        var value = _cache[key];
        
        _cache.Remove(key);
        
        return value;
    }
    
    public void Dispose()
    {
        _networkStream.Dispose();
        _writer.Dispose(); // TODO: Overhead?
        _reader.Dispose(); // TODO: Overhead?
    }
}
