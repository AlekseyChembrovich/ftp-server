namespace FtpServer.Files;

internal interface IDirectoryPositionTracker
{
    string CurrentPath { get; }
    
    void Change(string path);
    
    string GetPath(string path);
}

internal class DirectoryPositionTracker : IDirectoryPositionTracker
{
    private const string RootDir = "root";
    private static readonly string RootPath = Path.Combine(Environment.CurrentDirectory, RootDir);
    
    public string CurrentPath { get; private set; } = Path.Combine(RootPath, string.Empty);
    
    public DirectoryPositionTracker()
    {
        if (!Path.Exists(RootPath))
        {
            _ = Directory.CreateDirectory(RootPath);
        }
    }
    
    public void Change(string path)
    {
        path = path.TrimStart('/');
        
        CurrentPath = Path.Combine(RootPath, path);
    }
    
    public string GetPath(string path)
    {
        path = path.TrimStart('/');
        
        return Path.Combine(RootPath, path);
    }
}
