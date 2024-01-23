using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using FtpServer.Connection;

namespace FtpServer;

internal class MyFtpServer : IDisposable
{
    private readonly TcpListener _listener;
    
    private readonly IConnectionsFactory _connectionsFactory;
    private readonly IConnectionsPool _connectionsPool;

    public MyFtpServer(IConnectionsFactory connectionsFactory, IConnectionsPool connectionsPool)
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
                var networkStream = client.GetStream();

                var newConnection = _connectionsFactory.Create(networkStream);
                
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
        throw new NotImplementedException();
    }
}
