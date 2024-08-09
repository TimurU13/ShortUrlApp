namespace Crawler.Interfaces
{
    public interface ICustomQueue
    {
        void Enqueue(string url);
        string Dequeue();
        bool HasPendingTasks();
    }
}