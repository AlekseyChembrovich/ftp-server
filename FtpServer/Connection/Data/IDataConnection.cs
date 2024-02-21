using System.Net;

namespace FtpServer.Connection.Data;

public interface IDataConnection : IDisposable
{
    IPEndPoint DesignatedEndpoint { get; }
    
    Task OpenAsync(CancellationToken token = default);
    
    Task SendAsync(string line, CancellationToken token = default);
    
    Stream GetStream();
    
    void Close();
}
