namespace FtpServer;

internal enum FtpCommandType
{
    AUTH,
    USER,
    PASS,
    TYPE,
    FEAT,
    SYST,
    PWD,
    CWD,
    PASV,
    LIST,
    RETR,
    RNFR,
    RNTO,
    STOR,
    DELE,
    MKD,
    SIZE
}

internal enum TransferType
{
    Ascii,
    Binary
}

internal record struct FtpCommand(FtpCommandType Type, string Value);
