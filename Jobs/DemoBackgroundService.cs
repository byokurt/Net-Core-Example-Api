using System;
using System.Threading;
using System.Threading.Tasks;
using Medallion.Threading;
using Medallion.Threading.Redis;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DotnetCoreExampleApi.Jobs
{
    public class DemoBackgroundService : BackgroundService
    {
        private readonly TimeSpan _period = TimeSpan.FromSeconds(5);

        private readonly ILogger<DemoBackgroundService> _logger;

        private readonly RedisDistributedSynchronizationProvider _distributedSynchronizationProvider;

        public DemoBackgroundService(ILogger<DemoBackgroundService> logger, RedisDistributedSynchronizationProvider distributedSynchronizationProvider)
        {
            _logger = logger;
            _distributedSynchronizationProvider = distributedSynchronizationProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            using PeriodicTimer periodicTimer = new PeriodicTimer(_period);

            while (!cancellationToken.IsCancellationRequested && await periodicTimer.WaitForNextTickAsync(cancellationToken))
            {
                await Execute(cancellationToken);
            }
        }

        private async Task Execute(CancellationToken cancellationToken)
        {
            using (await _distributedSynchronizationProvider.AcquireLockAsync("DemoBackgroundService", null, cancellationToken))
            {
                _logger.LogInformation("Running process");
            }
        }
    }
}

