using Crawler.Interfaces;
using HtmlAgilityPack;
using System.Collections.Concurrent;
namespace Crawler;
public class WorkerPool : IWorkerPool
{
    private readonly ConcurrentQueue<Func<Task>> _tasks;
    private readonly SemaphoreSlim _semaphore;
    private readonly ILogger<WorkerPool> _logger;
    public WorkerPool(ILogger<WorkerPool> logger)
    {
        _tasks = new ConcurrentQueue<Func<Task>>();
        _semaphore = new SemaphoreSlim(3);
        _logger = logger;
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
                using var httpClient = new HttpClient();
                var html = await httpClient.GetStringAsync(url);
                var document = new HtmlDocument();
                document.LoadHtml(html);
                var images = document.DocumentNode.SelectNodes("//img");
                var imageUrls = images?.Select(img => img.GetAttributeValue("src", string.Empty)).Distinct();

                if (imageUrls == null || !imageUrls.Any())
                {
                    _logger.LogInformation("No images");
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
                            var imageData = await httpClient.GetByteArrayAsync(absoluteImageUrl);
                            var fileName = Path.GetFileName(new Uri(absoluteImageUrl).LocalPath);
                            var imagePath = Path.Combine(savePath, fileName);
                            await File.WriteAllBytesAsync(imagePath, imageData);
                        }
                    }
                    catch (Exception imageEx)
                    {
                        _logger.LogError(imageEx, $"Cannot download image {imageUrl}");
                    }
                }
            }
            catch (HttpRequestException httpEx)
            {
                _logger.LogError(httpEx, $"Http error.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Url error {url}");
            }
            finally
            {
                ReleaseWorker();
            }
        });

        await ProcessTasksAsync();
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
                _logger.LogError(ex, $"Task error");
            }
        }
    }
    public bool HasAvailableWorker() => _semaphore.CurrentCount > 0;
    public void ReleaseWorker()
    {
        _semaphore.Release();
    }
}