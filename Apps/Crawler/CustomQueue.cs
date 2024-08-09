using Crawler.Interfaces;
using System.Collections.Concurrent;
namespace Crawler
{
    public class CustomQueue : ICustomQueue
    {
        private readonly ConcurrentQueue<string> _queue;

        public CustomQueue()
        {
            _queue = new ConcurrentQueue<string>();
        }
        public void Enqueue(string url)
        {
            _queue.Enqueue(url);
        }

        public string Dequeue()
        {
            _queue.TryDequeue(out var url);
            return url;
        }
        public bool HasPendingTasks() => !_queue.IsEmpty;
    }
}