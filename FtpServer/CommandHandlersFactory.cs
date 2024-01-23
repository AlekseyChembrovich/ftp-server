using System.Collections.Generic;
using FtpServer.Files;
using FtpServer.Handlers;
using FtpServer.Handlers.Rename;

namespace FtpServer;

internal interface ICommandHandlersFactory
{
    IFtpCommandHandler Create(FtpCommandType commandType);
}

internal class CommandHandlersFactory : ICommandHandlersFactory
{
    private readonly IDictionary<FtpCommandType, IFtpCommandHandler> _commandHandlersMap =
        new Dictionary<FtpCommandType, IFtpCommandHandler>
        {
            { FtpCommandType.TYPE, new TypeCommandHandler() },
            { FtpCommandType.CWD, new ChangeDirectoryCommandHandler() },
            { FtpCommandType.PASV, new PassiveModeCommandHandler() },
            { FtpCommandType.LIST, new ListCommandHandler(new FilesRepository()) },
            { FtpCommandType.RETR, new RetrieveCommandHandler() },
            { FtpCommandType.RNFR, new RenameFromCommandHandler() },
            { FtpCommandType.RNTO, new RenameToCommandHandler() },
            { FtpCommandType.STOR, new StoreCommandHandler(new FilesRepository()) },
            { FtpCommandType.DELE, new DeleteCommandHandler() },
            { FtpCommandType.MKD, new MakeDirectoryCommandHandler() }
        };
    
    public IFtpCommandHandler Create(FtpCommandType commandType)
    {
        var isFound = _commandHandlersMap.TryGetValue(commandType, out var handler);
        
        handler = isFound
            ? handler
            : new DefaultFtpCommandHandler();
        
        return handler!;
    }
}
