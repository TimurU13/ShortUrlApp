using Microsoft.Extensions.Hosting;
using ShortUrlAppWebAPI.DAL;
using System.Threading;
using System.Threading.Tasks;

namespace ShortUrlAppWebAPI.Services
{
    public class HostedService : IHostedService
    {
        private readonly IUrlDataStore _urlDataStore;

        public HostedService(IUrlDataStore urlDataStore)
        {
            _urlDataStore = urlDataStore;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            (_urlDataStore as IDisposable)?.Dispose();
            return Task.CompletedTask;
        }
    }
}