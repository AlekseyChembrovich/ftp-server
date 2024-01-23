using System;
using System.IO;

namespace FtpServer.Files;

internal interface IPositionTracker
{
    string CurrentPath { get; }
    
    void Change(string path);
    
    string GetPath(string path);
}

internal class PositionTracker : IPositionTracker
{
    private const string RootDir = "root";
    private static readonly string RootPath = Path.Combine(Environment.CurrentDirectory, RootDir);
    
    public string CurrentPath { get; private set; } = Path.Combine(RootPath, string.Empty);
    
    public PositionTracker()
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
