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
    PORT,
    PASV,
    LIST,
    RETR,
    RNFR,
    RNTO,
    STOR,
    DELE,
    MKD,
    SIZE,
    QUIT,
    NONE
}

internal record struct FtpCommand(FtpCommandType Type, string Value);
