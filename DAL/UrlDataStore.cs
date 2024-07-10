namespace ShortUrlSrv.App.DAL;

internal class UrlDataStore : IUrlDataStore
{
    private Dictionary<int, string> _urlStore = new();
    private int _currentKey = 0;

    public string SaveLongID(string longID)
    {
        if (string.IsNullOrEmpty(longID))
            throw new Exception("Ссылка пустая!");
        if (_urlStore.ContainsKey(_currentKey))
            _urlStore.Add(_currentKey, longID);
        else
            _urlStore.Add(++_currentKey, longID);
        return _currentKey.ToString();

    }

    public string GetLongID(string shortID)
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
