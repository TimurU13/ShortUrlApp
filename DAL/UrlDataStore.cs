using System.IO;
using System.Text;
using System.Text.Json;

namespace ShortUrlSrv.App.DAL;

internal class UrlDataStore : IUrlDataStore
{
    private Dictionary<int, string> _urlStore;
    private int _currentKey;
    private string _path;

    public UrlDataStore(string path)
    {
        _urlStore = new Dictionary<int, string>();
        _currentKey = 0;
        _path = path;

        if (!File.Exists(_path))
        {
            File.Create(_path);
        }
        else
        {
            string json = File.ReadAllText(_path);
            if (json.Length > 0)
            {
                _urlStore = JsonSerializer.Deserialize<Dictionary<int, string>>(json) ?? new Dictionary<int, string>();
                _currentKey = _urlStore.Keys.Last();
            }            
        }
    }

    public void Dispose()
    {
        string json = JsonSerializer.Serialize(_urlStore);
        File.WriteAllText(_path, json);
    }

    public string SaveLongUrl(string longUrl)
    {
        if (string.IsNullOrEmpty(longUrl))
            throw new Exception("Ссылка пустая!");
        if (_urlStore.ContainsKey(_currentKey))
            _urlStore.Add(_currentKey, longUrl);
        else
            _urlStore.Add(++_currentKey, longUrl);
        return _currentKey.ToString();

    }

    public (string, bool) GetLongUrl(string shortID)
    {

        int searchedKey = int.Parse(shortID);
        if (searchedKey > _currentKey)
           return (null, false);
        return (_urlStore[searchedKey], true);

    }


    public bool DeleteShortID(string shortID)
    {
        int searchedKey = int.Parse(shortID);
        if (searchedKey > _currentKey)
            return false;
        else
            _urlStore.Remove(searchedKey);
        return true;

    }

}
