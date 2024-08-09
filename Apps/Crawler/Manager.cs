using Crawler.Interfaces;
using System.Collections.Concurrent;
namespace Crawler;
public class Manager : IManager
{
    private readonly IWorkerPool _workerPool;
    private readonly ICustomQueue _queue;
    private readonly IArchivator _archivator;
    private readonly ConcurrentDictionary<string, (Status Status, string FilePath)> _requests;
    private readonly ILogger<Manager> _logger;

    public Manager(IWorkerPool workerPool, ICustomQueue queue, IArchivator archivator, ILogger<Manager> logger)
    {
        _workerPool = workerPool;
        _queue = queue;
        _archivator = archivator;
        _requests = new ConcurrentDictionary<string, (Status Status, string FilePath)>();
        _logger = logger;
    }

    public string ImageRequest(string url)
    {
        string requestId = Guid.NewGuid().ToString();
        _queue.Enqueue(url);
        _requests[requestId] = (Status.InProgress, null);

        Task.Run(async () =>
        {
            while (_queue.HasPendingTasks())
            {
                if (_workerPool.HasAvailableWorker())
                {
                    string siteUrl = _queue.Dequeue();
                    string savePath = Path.Combine("C:\\Crawler", requestId);

                    try
                    {
                        await _workerPool.StartWorkAsync(siteUrl, savePath);
                        _requests[requestId] = (Status.Completed, savePath);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, $"Error processing URL {siteUrl}");
                        _requests[requestId] = (Status.Failed, null);
                    }
                }
                await Task.Delay(100);
            }
        });
        return requestId;
    }

    public (Status Status, string DownloadUrl) GetStatus(string statusId)
    {
        if (_requests.TryGetValue(statusId, out var requestInfo))
        {
            if (requestInfo.Status == Status.Completed)
            {
                try
                {
                    string archiveUrl = _archivator.Create(requestInfo.FilePath);
                    return (Status.Completed, archiveUrl);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error create archive");
                    return (Status.Completed, null);
                }
            }
            return (requestInfo.Status, null);
        }
        return (Status.NotFound, null);
    }
}
public enum Status
{
    InProgress,
    Completed,
    Failed,
    NotFound
}