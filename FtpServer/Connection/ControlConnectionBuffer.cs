namespace FtpServer.Connection;

internal interface IControlConnectionBuffer
{
    void SetCacheValue(string key, string value);
    
    string GetCacheValue(string key);
}

public class ControlConnectionBuffer : IControlConnectionBuffer
{
    private readonly IDictionary<string, string> _cache = new Dictionary<string, string>();
    
    public void SetCacheValue(string key, string value)
    {
        _cache[key] = value;
    }
    
    public string GetCacheValue(string key)
    {
        var value = _cache[key];
        
        _cache.Remove(key);
        
        return value;
    }
}
