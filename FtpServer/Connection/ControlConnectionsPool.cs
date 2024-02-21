using System.Threading.Channels;

namespace FtpServer.Connection;

internal interface IControlConnectionsPool : IDisposable
{
    Task PassConnectionAsync(IControlConnection connection, CancellationToken token = default);
}

internal class ControlConnectionsPool : IControlConnectionsPool
{
    private readonly Channel<IControlConnection> _connectionsChannel;
    private readonly Task _connectionsProcessingTask;
    
    private readonly CancellationTokenSource _cancellationTokenSource = new();
    private readonly CancellationToken _cancellationToken;
    
    public ControlConnectionsPool()
    {
        _connectionsChannel = Channel.CreateUnbounded<IControlConnection>();   
        _connectionsProcessingTask = ProcessConnectionsAsync();
        _cancellationToken = _cancellationTokenSource.Token;
    }
    
    public Task PassConnectionAsync(IControlConnection connection, CancellationToken token = default)
        => _connectionsChannel.Writer
            .WriteAsync(connection, token)
            .AsTask();
    
    private async Task ProcessConnectionsAsync()
    {
        var stream = _connectionsChannel.Reader.ReadAllAsync(_cancellationToken);
        
        await Parallel.ForEachAsync(
            stream,
            _cancellationToken,
            async (connection, token) => await connection.BeginInteractionAsync(token)
        );
    }
    
    public void Dispose()
    {
        _cancellationTokenSource.Cancel();
        
        _connectionsProcessingTask.Dispose();
    }
}
