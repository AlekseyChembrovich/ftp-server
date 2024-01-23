using System;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace FtpServer.Connection;


internal interface IConnectionsPool : IDisposable
{
    Task PassConnectionAsync(IFtpConnection connection, CancellationToken token = default);
}

internal class DefaultConnectionsPool : IConnectionsPool
{
    private readonly Channel<IFtpConnection> _connectionsChannel;
    private readonly Task _connectionsProcessingTask;
    
    private readonly CancellationTokenSource _cancellationTokenSource = new();
    private readonly CancellationToken _cancellationToken;

    public DefaultConnectionsPool()
    {
        _connectionsChannel = Channel.CreateUnbounded<IFtpConnection>();   
        _connectionsProcessingTask = ProcessConnectionsAsync();
        _cancellationToken = _cancellationTokenSource.Token;
    }

    public Task PassConnectionAsync(IFtpConnection connection, CancellationToken token = default) =>
        _connectionsChannel.Writer.WriteAsync(connection, token).AsTask();

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
