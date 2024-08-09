using Crawler.Interfaces;
using HtmlAgilityPack;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
namespace Crawler
{
    public class WorkerPool : IWorkerPool
    {
        private readonly ConcurrentQueue<Func<Task>> _tasks;
        private readonly SemaphoreSlim _semaphore;
        private readonly ILogger<WorkerPool> _logger;
        private readonly HttpClient _httpClient;

        public WorkerPool(ILogger<WorkerPool> logger)
        {
            _tasks = new ConcurrentQueue<Func<Task>>();
            _semaphore = new SemaphoreSlim(3);
            _logger = logger;
            _httpClient = new HttpClient();
        }

        public void InitializeWorkers(int workerCount)
        {
            _semaphore.Release(workerCount);
        }

        public async Task StartWorkAsync(string url, string savePath)
        {
            await _semaphore.WaitAsync();

            _tasks.Enqueue(async () =>
            {
                try
                {
                    await CrawlSiteAsync(url, savePath);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error processing URL {url}");
                }
                finally
                {
                    ReleaseWorker();
                }
            });

            await ProcessTasksAsync();
        }

        private async Task CrawlSiteAsync(string startUrl, string savePath)
        {
            var visitedUrls = new HashSet<string>();
            var urlsToVisit = new ConcurrentQueue<string>();
            urlsToVisit.Enqueue(startUrl);

            while (urlsToVisit.TryDequeue(out var url))
            {
                if (!visitedUrls.Add(url))
                {
                    continue;
                }

                await DownloadImagesFromPageAsync(url, savePath);

                var internalUrls = await GetInternalLinksAsync(url, startUrl);
                foreach (var internalUrl in internalUrls)
                {
                    urlsToVisit.Enqueue(internalUrl);
                }
            }
        }
        private async Task DownloadImagesFromPageAsync(string url, string savePath)
        {
            try
            {
                var html = await _httpClient.GetStringAsync(url);
                var document = new HtmlDocument();
                document.LoadHtml(html);
                var images = document.DocumentNode.SelectNodes("//img");
                var imageUrls = images?.Select(img => img.GetAttributeValue("src", string.Empty)).Distinct();

                if (imageUrls == null || !imageUrls.Any())
                {
                    _logger.LogInformation($"No images found on {url}");
                    return;
                }

                if (!Directory.Exists(savePath))
                {
                    Directory.CreateDirectory(savePath);
                }

                foreach (var imageUrl in imageUrls)
                {
                    try
                    {
                        var absoluteImageUrl = new Uri(new Uri(url), imageUrl).ToString();
                        if (absoluteImageUrl.StartsWith("http://") || absoluteImageUrl.StartsWith("https://"))
                        {
                            var imageData = await _httpClient.GetByteArrayAsync(absoluteImageUrl);
                            var fileName = Path.GetFileName(new Uri(absoluteImageUrl).LocalPath);
                            var imagePath = Path.Combine(savePath, fileName);
                            await File.WriteAllBytesAsync(imagePath, imageData);
                        }
                    }
                    catch (Exception imageEx)
                    {
                        _logger.LogError(imageEx, $"Cannot download image {imageUrl} from {url}");
                    }
                }
            }
            catch (HttpRequestException httpEx)
            {
                _logger.LogError(httpEx, $"HTTP request error for {url}");
            }
        }

        private async Task<IEnumerable<string>> GetInternalLinksAsync(string url, string baseDomain)
        {
            var internalLinks = new List<string>();
            try
            {
                var html = await _httpClient.GetStringAsync(url);
                var document = new HtmlDocument();
                document.LoadHtml(html);
                var links = document.DocumentNode.SelectNodes("//a[@href]");
                var baseUri = new Uri(baseDomain);
                foreach (var link in links)
                {
                    var href = link.GetAttributeValue("href", string.Empty);
                    if (Uri.TryCreate(href, UriKind.Absolute, out var linkUri))
                    {
                        if (linkUri.Host == baseUri.Host)
                            internalLinks.Add(linkUri.ToString());
                    }
                    else
                    {
                        var absoluteUri = new Uri(baseUri, href);
                        if (absoluteUri.Host == baseUri.Host)
                            internalLinks.Add(absoluteUri.ToString());
                    }
                }
            }
            catch (HttpRequestException httpEx)
            {
                _logger.LogError(httpEx, $"Error fetching links from {url}");
            }
            return internalLinks.Distinct();
        }

        private async Task ProcessTasksAsync()
        {
            while (_tasks.TryDequeue(out var task))
            {
                try
                {
                    await task();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Task error");
                }
            }
        }
        public bool HasAvailableWorker() => _semaphore.CurrentCount > 0;

        public void ReleaseWorker()
        {
            _semaphore.Release();
        }
    }
}