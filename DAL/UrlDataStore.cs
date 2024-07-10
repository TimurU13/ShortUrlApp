namespace ShortUrlSrv.App.DAL;

internal class UrlDataStore : IUrlDataStore
{
    private Dictionary<int, string> _urlStore = new();
    private int _currentKey = 0;

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

    public string GetLongUrl(string shortID)
    {

        int searchedKey = int.Parse(shortID);
        if (searchedKey > _currentKey)
            throw new IndexOutOfRangeException();
        return _urlStore[searchedKey];

    }


    public bool DeleteShortID(string shortID)
    {
        int searchedKey = int.Parse(shortID);
        if (searchedKey > _currentKey)
            throw new IndexOutOfRangeException();
        else
            _urlStore.Remove(searchedKey);
        return true;

    }

}
