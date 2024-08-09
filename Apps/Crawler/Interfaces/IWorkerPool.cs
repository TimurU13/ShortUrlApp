namespace Crawler.Interfaces
{
    public interface IWorkerPool
    {
        void InitializeWorkers(int workerCount);
        Task StartWorkAsync(string url, string savePath);
        bool HasAvailableWorker();
        void ReleaseWorker();
    }
}