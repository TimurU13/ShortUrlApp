namespace ShortUrlAppWebAPI.DAL;
public interface IUrlDataStore : IDisposable
{
    public bool DeleteShortID(string shortID);

    public string SaveLongUrl(string longID);

    public (string, bool) GetLongUrl(string shortID);
}