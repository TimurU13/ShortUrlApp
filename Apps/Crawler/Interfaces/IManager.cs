namespace Crawler.Interfaces
{
    public interface IManager
    {
        string ImageRequest(string url);
        (Status Status, string DownloadUrl) GetStatus(string statusId);
    }
}