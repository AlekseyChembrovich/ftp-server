using System;
using System.IO;
using System.Net;
using System.Text;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace FtpServer.Connection;

internal interface IPassiveSession : IDisposable
{
    TcpListener Listener { get; }
    
    void Start();
    
    Task InitInteractionAsync(CancellationToken token = default);
    
    Task SendAsync(string response, CancellationToken token = default);

    Stream GetStream();
}

internal class PassiveSession : IPassiveSession
{
    public TcpListener Listener { get; private set; }

    private NetworkStream _networkStream;
    private StreamWriter _writer;

    public void Start()
    {
        Listener = new TcpListener(IPAddress.Any, 0);
        Listener.Start();
    }

    public async Task InitInteractionAsync(CancellationToken token = default)
    {
        var client = await Listener.AcceptTcpClientAsync(token);
        _networkStream = client.GetStream();
        _networkStream.Socket.LingerState = new LingerOption(true, 0); // Non TIME_WAIT
        _writer = new StreamWriter(_networkStream, Encoding.ASCII);
    }
    
    public async Task SendAsync(string response, CancellationToken token = default)
    {
        await _writer.WriteLineAsync(response);
        await _writer.FlushAsync(token);
    }
    
    public Stream GetStream() => _networkStream;
    
    public void Dispose()
    {
        _writer.Dispose();
        Listener.Stop();
        Listener.Dispose();
    }
}
