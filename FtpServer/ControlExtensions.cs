using System;
using System.Linq;

namespace FtpServer;

internal static class ControlExtensions
{
    public static string ToResponse(this FtpCommand command) => command.Type switch
    {
        FtpCommandType.USER => $"331 Login {command.Value} is correct.",
        FtpCommandType.PASS => $"230 Password {command.Value} is correct.",
        // FtpCommandType.TYPE => $"200 Type {command.Value} is set.",
        FtpCommandType.SYST => "215 Windows_NT",
        FtpCommandType.PWD => "257 /",
        // FtpCommandType.CWD => $"250 {command.Value} is the current directory.",
        _ => $"502 Command {command.Type.ToString()} not implemented."
    };

    public static FtpCommand ParseFtpCommand(this string line)
    {
        var pair = line.Split(' ');
        var type = ParseFtpCommandType(pair.First());

        return new FtpCommand(type, pair.Last());
    }

    public static TransferType ParseTransferType(this string value) => value switch
    {
        "A" => TransferType.Ascii,
        "I" => TransferType.Binary,
        _ => throw new ArgumentException($"Unsupported transfer type: {value}.")
    };

    private static FtpCommandType ParseFtpCommandType(string command) => command switch
    {
        "AUTH" => FtpCommandType.AUTH,
        "USER" => FtpCommandType.USER,
        "PASS" => FtpCommandType.PASS,
        "TYPE" => FtpCommandType.TYPE,
        "FEAT" => FtpCommandType.FEAT,
        "SYST" => FtpCommandType.SYST,
        "PWD" => FtpCommandType.PWD,
        "CWD" => FtpCommandType.CWD,
        "PASV" => FtpCommandType.PASV,
        "LIST" => FtpCommandType.LIST,
        "RETR" => FtpCommandType.RETR,
        "RNFR" => FtpCommandType.RNFR,
        "RNTO" => FtpCommandType.RNTO,
        "STOR" => FtpCommandType.STOR,
        "DELE" => FtpCommandType.DELE,
        "MKD" => FtpCommandType.MKD,
        "SIZE" => FtpCommandType.SIZE,
        _ => throw new ArgumentException($"Unsupported command type: {command}.")
    };
}
