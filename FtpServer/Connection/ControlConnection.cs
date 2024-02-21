using System.Net;
using System.Text;
using FtpServer.Files;
using System.Net.Sockets;
using FtpServer.Connection.Data;
using FtpServer.Handlers.Type;

namespace FtpServer.Connection;

internal interface IControlConnection : IDisposable
{
    IDirectoryPositionTracker PositionTracker { get; }
    IControlConnectionBuffer ConnectionBuffer { get; }
    
    Task BeginInteractionAsync(CancellationToken token = default);
    Task SendAsync(string response, CancellationToken token = default);
    void Close();
    
    CodingType CodingType { get; }
    void SetupCodingType(CodingType codingType);
    
    IDataConnection DataConnection { get; }
    void SetupDataConnection(DataConnectionMode connectionMode, IPAddress ipAddress, int port);
}

internal class ControlConnection : IControlConnection
{
    private readonly TcpClient _connectionClient;
    private readonly StreamWriter _controlWriter;
    private readonly StreamReader _controlReader;
    
    private readonly ICommandHandlersFactory _handlersFactory;
    public IDirectoryPositionTracker PositionTracker { get; }
    public IControlConnectionBuffer ConnectionBuffer { get; }
    
    public ControlConnection(
        TcpClient client,
        ICommandHandlersFactory handlersFactory,
        IDirectoryPositionTracker positionTracker,
        IControlConnectionBuffer connectionBuffer)
    {
        _connectionClient = client;
        var networkStream = _connectionClient.GetStream();
        _controlWriter = new StreamWriter(networkStream, Encoding.ASCII);
        _controlReader = new StreamReader(networkStream, Encoding.ASCII);
        
        _handlersFactory = handlersFactory;
        PositionTracker = positionTracker;
        ConnectionBuffer = connectionBuffer;
    }
    
    public async Task BeginInteractionAsync(CancellationToken token = default)
    {
        try
        {
            await SendAsync("220 Welcome to the FTP Server.", token);
            
            string? line;
            while (!string.IsNullOrWhiteSpace(line = await _controlReader.ReadLineAsync(token)))
            {
                Console.WriteLine(line);
                var ftpCommand = line.ParseFtpCommand();
                
                if (ftpCommand.Type is FtpCommandType.QUIT)
                {
                    Close();
                    
                    break;
                }
                
                var handler = _handlersFactory.Create(ftpCommand.Type);
                var response = await handler!.HandleAsync(ftpCommand, this, token);
                
                await SendAsync(response, token);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[{ex.GetType()}] at Ftp connection level." +
                              $" An error occurred with message: {ex.Message}" +
                              $" and inner message: {ex.InnerException?.Message}");
            
            throw;
        }
    }
    
    public async Task SendAsync(string response, CancellationToken token = default)
    {
        await _controlWriter.WriteLineAsync(response);
        await _controlWriter.FlushAsync(token);
    }
    
    #region Coding type
    
    private CodingType? _codingType;
    public CodingType CodingType
    {
        get
        {
            if (_codingType is null)
            {
                ThrowNoCodingTypeException();
            }
            
            return _codingType!.Value;
        }
    }
    
    public void SetupCodingType(CodingType codingType)
    {
        _codingType = codingType;
    }
    
    #endregion
    
    #region Data connection
    
    private IDataConnection? _dataConnection;
    public IDataConnection DataConnection
    {
        get
        {
            if (_dataConnection is null)
            {
                ThrowNoDataConnectionException();
            }
            
            return _dataConnection!;
        }
    }
    
    public void SetupDataConnection(DataConnectionMode connectionMode, IPAddress ipAddress, int port)
    {
        _dataConnection = connectionMode is DataConnectionMode.Active
            ? new ActiveModeConnection(ipAddress, port)
            : new PassiveModeConnection(ipAddress, port);
    }
    
    
    #endregion
    
    public void Close()
    {
        _controlWriter.Close();
        _controlReader.Close();
        _connectionClient.Close();
    }
    
    public void Dispose()
    {
        _controlWriter.Dispose();
        _controlReader.Dispose();
        _connectionClient.Dispose();
    }
    
    #region Exceptions
    
    private static void ThrowNoCodingTypeException()
    {
        throw new Exception("Coding type was not setup.");
    }
    
    private static void ThrowNoDataConnectionException()
    {
        throw new Exception("Data connection was not established.");
    }
    
    #endregion
}
