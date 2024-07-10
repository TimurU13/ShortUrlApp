namespace ShortUrlSrv.App.DAL;

public interface IUrlDataStore
{
    public bool DeleteShortID(string shortID);

    public string SaveLongUrl(string longID);

    public string GetLongUrl(string shortID);
}