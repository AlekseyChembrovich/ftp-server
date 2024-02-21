using System.Net;
using System.Net.Sockets;
using System.Text;

namespace FtpServer.Connection.Data;

internal class PassiveModeConnection : IDataConnection
{
    private readonly TcpListener _listener;
    private TcpClient? _client;
    private NetworkStream? _networkStream;
    private StreamWriter? _writer;
    
    public IPEndPoint DesignatedEndpoint { get; }
    
    public PassiveModeConnection(IPAddress ipAddress, int port)
    {
        _listener = new TcpListener(IPAddress.Any, 0);
        _listener.Start();
        DesignatedEndpoint = (IPEndPoint)_listener.LocalEndpoint;
    }
    
    public async Task OpenAsync(CancellationToken token = default)
    {
        _listener.Start();
        _client = await _listener.AcceptTcpClientAsync(token);
        
        _networkStream = _client.GetStream();
        _networkStream.Socket.LingerState = new LingerOption(true, 0); // Non TIME_WAIT
        _writer = new StreamWriter(_networkStream, Encoding.ASCII);
    }
    
    public async Task SendAsync(string line, CancellationToken token = default)
    {
        if (_writer is null)
        {
            ThrowNoConnectionException();
        }
        
        await _writer!.WriteLineAsync(line);
        await _writer!.FlushAsync(token);
    }
    
    public Stream GetStream()
    {
        if (_networkStream is null)
        {
            ThrowNoConnectionException();
        }
        
        return _networkStream!;
    }
    
    public void Close()
    {
        if (_networkStream is null
            || _writer is null
            || _client is null)
        {
            ThrowNoConnectionException();
        }
        
        _networkStream!.Close();
        _writer!.Close();
        _client!.Close();
        _listener!.Stop();
    }
    
    public void Dispose()
    {
        if (_networkStream is null
            || _writer is null
            || _client is null)
        {
            ThrowNoConnectionException();
        }
        
        _networkStream!.Dispose();
        _writer!.Dispose();
        _client!.Dispose();
        _listener!.Dispose();
    }
    
    private static void ThrowNoConnectionException()
    {
        throw new Exception("Passive mode connection was not established.");
    }
}
