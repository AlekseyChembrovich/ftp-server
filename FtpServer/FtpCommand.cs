namespace FtpServer;

internal enum FtpCommandType
{
    OPTS, // TODO: need to be implemented
    AUTH,
    USER,
    PASS,
    TYPE,
    FEAT,
    SYST,
    PWD,
    CWD,
    PORT, // TODO: need to be implemented
    PASV,
    LIST,
    RETR,
    RNFR,
    RNTO,
    STOR,
    DELE,
    MKD,
    SIZE,
    NONE
}

internal enum TransferType
{
    Ascii,
    Binary
}

internal record struct FtpCommand(FtpCommandType Type, string Value);
