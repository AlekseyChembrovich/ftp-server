using FtpServer.Handlers.Type;

namespace FtpServer;

internal static class ControlExtensions
{
    public static string ToResponse(this FtpCommand command) => command.Type switch
    {
        FtpCommandType.USER => $"331 Login {command.Value} is correct.",
        FtpCommandType.PASS => $"230 Password {command.Value} is correct.",
        FtpCommandType.SYST => "215 Windows_NT",
        FtpCommandType.PWD => "257 /",
        _ => $"502 Command {command.Type.ToString()} not implemented."
    };
    
    public static FtpCommand ParseFtpCommand(this string line)
    {
        var pair = line.Split(' ');
        var type = ParseFtpCommandType(pair.First());
        
        return new FtpCommand(type, pair.Last());
    }
    
    public static CodingType ParseCodingType(this string value) => value switch
    {
        "A" => CodingType.Ascii,
        "I" => CodingType.Binary,
        _ => throw new ArgumentException($"Unsupported coding type: {value}.")
    };
    
    private static FtpCommandType ParseFtpCommandType(string command) => command.ToLower() switch
    {
        "opts" => FtpCommandType.OPTS,
        "auth" => FtpCommandType.AUTH,
        "user" => FtpCommandType.USER,
        "pass" => FtpCommandType.PASS,
        "type" => FtpCommandType.TYPE,
        "feat" => FtpCommandType.FEAT,
        "syst" => FtpCommandType.SYST,
        "pwd" => FtpCommandType.PWD,
        "cwd" => FtpCommandType.CWD,
        "port" => FtpCommandType.PORT,
        "pasv" => FtpCommandType.PASV,
        "list" => FtpCommandType.LIST,
        "retr" => FtpCommandType.RETR,
        "rnfr" => FtpCommandType.RNFR,
        "rnto" => FtpCommandType.RNTO,
        "stor" => FtpCommandType.STOR,
        "dele" => FtpCommandType.DELE,
        "mkd" => FtpCommandType.MKD,
        "size" => FtpCommandType.SIZE,
        _ => FtpCommandType.NONE
    };
}
