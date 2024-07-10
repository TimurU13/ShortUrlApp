namespace ShortUrlSrv.App.DAL;

public interface IUrlDataStore
{
    public bool DeleteShortID(string shortID);

    public string SaveLongID(string longID);

    public string GetLongID(string shortID);
}