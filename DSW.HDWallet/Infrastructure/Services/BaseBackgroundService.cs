using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NCrontab;

namespace DSW.HDWallet.Infrastructure.Services
{
    public abstract class BaseBackgroundService<T> : BackgroundService
    where T : BackgroundService
    {
        protected readonly ILogger<T> logger;
        protected readonly CrontabSchedule schedule;

        private readonly string serviceName;

        public BaseBackgroundService(
            ILogger<T> logger,
            string schedule
        )
        {
            this.logger = logger;

            serviceName = this.GetType().Name;

            this.schedule = CrontabSchedule.Parse(
                schedule,
                new()
                {
                    IncludingSeconds = true
                }
            );
        }

        protected virtual Task OnStarting(CancellationToken cancellationToken) => Task.CompletedTask;

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            logger.LogTrace("{serviceName} starting.", serviceName);

            await base.StartAsync(cancellationToken);

            await this.OnStarting(cancellationToken);

            logger.LogTrace("{serviceName} started.", serviceName);
        }

        protected abstract Task OnExecute(CancellationToken cancellationToken);

        protected override async Task ExecuteAsync(CancellationToken cancellationToken = default)
        {
            if (cancellationToken.IsCancellationRequested) return;

            logger.LogTrace("{serviceName} executing.", serviceName);

            await this.OnExecute(cancellationToken);

            logger.LogTrace("{serviceName} executed.", serviceName);
        }

        protected virtual Task OnStopping(CancellationToken cancellationToken) => Task.CompletedTask;

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            logger.LogTrace("{serviceName} stopping.", serviceName);

            await this.OnStopping(cancellationToken);

            await base.StopAsync(cancellationToken);

            logger.LogTrace("{serviceName} stopped.", serviceName);
        }

        protected virtual void OnDisposing() { }

        public override void Dispose()
        {
            logger.LogTrace("{serviceName} disposing.", serviceName);

            OnDisposing();

            base.Dispose();

            GC.SuppressFinalize(this);

            logger.LogTrace("{serviceName} disposed.", serviceName);
        }
    }
}