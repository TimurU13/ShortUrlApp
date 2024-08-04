namespace Crawler.Interfaces
{
    public interface IDownload
    {
        public Task<List<string>> GetImages(string url);
        public Task DownloadImages(List<string> imageUrls, string savePath);
    }
}
