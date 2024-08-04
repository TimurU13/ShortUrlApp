namespace Crawler.Interfaces
{
    public interface IManager
    {
        public string ImageRequest (string url);
        public (string, string) GetStatus(string statusId);
    }
}
