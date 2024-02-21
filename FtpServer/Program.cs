using FtpServer;
using FtpServer.Connection;

// Autofac

using var hostServer = new HostServer(
    new ControlConnectionsFactory(),
    new ControlConnectionsPool()
);

await hostServer.StartAsync();

Console.WriteLine("Program is completed.");
