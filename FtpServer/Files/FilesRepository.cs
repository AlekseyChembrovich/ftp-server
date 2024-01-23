using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FtpServer.Files;

internal interface IFilesRepository
{
    IEnumerable<string> GetListByPath(string path);
    
    Task SaveFileBinaryAsync(Stream inputStream, string path, CancellationToken token = default);

    Task SaveFileAsciiAsync(Stream inputStream, string path, CancellationToken token = default);
}

internal class FilesRepository : IFilesRepository
{
    public IEnumerable<string> GetListByPath(string path)
    {
        var directories = Directory.EnumerateDirectories(path)
            .Select(dir =>
            {
                var dirInfo = new DirectoryInfo(dir);
                var date = dirInfo.LastWriteTime < DateTime.Now - TimeSpan.FromDays(180)
                    ? dirInfo.LastWriteTime.ToString("MMM dd  yyyy")
                    : dirInfo.LastWriteTime.ToString("MMM dd HH:mm");

                var record = $"drwxr-xr-x 2 alex alex {"4096",8} {date} {dirInfo.Name}";
                
                return record;
            });

        var files = Directory.EnumerateFiles(path)
            .Select(file =>
            {
                var fileInfo = new FileInfo(file);
                var date = fileInfo.LastWriteTime < DateTime.Now - TimeSpan.FromDays(180)
                    ? fileInfo.LastWriteTime.ToString("MMM dd  yyyy")
                    : fileInfo.LastWriteTime.ToString("MMM dd HH:mm");

                var record = $"-rw-r--r-- 2 alex alex {fileInfo.Length,8} {date} {fileInfo.Name}";

                return record;
            })
            .ToList();

        return directories.Union(files).ToList();
    }

    public async Task SaveFileBinaryAsync(Stream inputStream, string path, CancellationToken token = default)
    {
        await using var fileStream = File.Create(path);
        var buffer = new byte[1024];
        int bytesRead;
        while ((bytesRead = await inputStream.ReadAsync(buffer, 0, buffer.Length, token)) > 0)
        {
            await fileStream.WriteAsync(buffer, 0, bytesRead, token);
        }
    }
    
    public async Task SaveFileAsciiAsync(Stream inputStream, string path, CancellationToken token = default)
    {
        await using var fileStream = File.Create(path);
        using var reader = new StreamReader(inputStream, Encoding.ASCII);
        await using var writer = new StreamWriter(fileStream, Encoding.ASCII);
        
        var buffer = new char[1024];
        int bytesRead;
        while ((bytesRead = await reader.ReadAsync(buffer, 0, buffer.Length)) > 0)
        {
            await writer.WriteAsync(buffer, 0, bytesRead);
        }
    }
}
