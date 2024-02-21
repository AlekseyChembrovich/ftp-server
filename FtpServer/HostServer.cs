using System.Net;
using System.Net.Sockets;
using FtpServer.Connection;

namespace FtpServer;

internal class HostServer : IDisposable
{
    private readonly TcpListener _listener;
    
    private readonly IControlConnectionsFactory _connectionsFactory;
    private readonly IControlConnectionsPool _connectionsPool;
    
    public HostServer(
        IControlConnectionsFactory connectionsFactory,
        IControlConnectionsPool connectionsPool)
    {
        _listener = new TcpListener(IPAddress.Any, 21);
        _listener.Start();
        
        _connectionsFactory = connectionsFactory;
        _connectionsPool = connectionsPool;
    }
    
    public async Task StartAsync(CancellationToken token = default)
    {
        try
        {
            Console.WriteLine("Listening...");
            while (!token.IsCancellationRequested)
            {
                var client = await _listener.AcceptTcpClientAsync(token);
                
                var ad1 = (IPEndPoint)client.Client.RemoteEndPoint;
                var ad2 = (IPEndPoint)client.Client.LocalEndPoint;
                
                Console.WriteLine($"Received a client. {ad1.Address.ToString()}:{ad1.Port}       {ad2.Address.ToString()}:{ad2.Port}");
                
                var newConnection = _connectionsFactory.Create(client);
                
                Console.WriteLine("New connection is created.");
                
                await _connectionsPool.PassConnectionAsync(newConnection, token);
                
                Console.WriteLine("Connection is passed to processing.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[{ex.GetType()}] at Ftp server level." +
                              $" An error occurred with message: {ex.Message}" +
                              $" and inner message: {ex.InnerException?.Message}");
            
            throw;
        }
    }
    
    public void Dispose()
    {
        _listener.Stop();
        _listener.Dispose();
        _connectionsPool.Dispose();
    }
}
