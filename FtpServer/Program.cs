using System;
using FtpServer;
using FtpServer.Connection;

// Autofac

using var ftpServer = new MyFtpServer(
    new FtpConnectionsFactory(),
    new DefaultConnectionsPool()
);

await ftpServer.StartAsync();

Console.WriteLine("Program is completed.");
