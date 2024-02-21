using System.Net;
using System.Net.Sockets;
using System.Text;

namespace FtpServer.Connection.Data;

internal class ActiveModeConnection : IDataConnection
{
    private readonly TcpClient _client;
    private NetworkStream? _networkStream;
    private StreamWriter? _writer;
    
    private readonly IPEndPoint? _endpoint;
    public IPEndPoint DesignatedEndpoint
    {
        get
        {
            if (_endpoint is null)
            {
                ThrowNoConnectionException();
            }
            
            return _endpoint!;
        }
    }
    
    public ActiveModeConnection(IPAddress ipAddress, int port)
    {
        _endpoint = new IPEndPoint(ipAddress, port);
        _client = new TcpClient();
    }
    
    public async Task OpenAsync(CancellationToken token = default)
    {
        await _client.ConnectAsync(DesignatedEndpoint, token);
        
        _networkStream = _client.GetStream();
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
            || _writer is null)
        {
            ThrowNoConnectionException();
        }
        
        _networkStream!.Close();
        _writer!.Close();
        _client!.Close();
    }
    
    public void Dispose()
    {
        if (_networkStream is null
            || _writer is null)
        {
            ThrowNoConnectionException();
        }
        
        _networkStream!.Dispose();
        _writer!.Dispose();
        _client!.Dispose();
    }
    
    private static void ThrowNoConnectionException()
    {
        throw new Exception("Active mode connection was not established.");
    }
}
