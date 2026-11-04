using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace OTTR.Services
{
   
    public class EmailNotificationCron : CronJobService
    {
        private readonly ILogger<EmailNotificationCron> _logger;
        private readonly IServiceProvider _serviceProvider;

        public EmailNotificationCron(IScheduleConfig<EmailNotificationCron> config, ILogger<EmailNotificationCron> logger, IServiceProvider serviceProvider)
            : base(config.CronExpression, config.TimeZoneInfo)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            return base.StartAsync(cancellationToken);
        }

        public override async Task DoWork(CancellationToken cancellationToken)
        {
            using var scope = _serviceProvider.CreateScope();
            var svc = scope.ServiceProvider.GetRequiredService<IEmailNotificationService>();
            await svc.DoWork(cancellationToken);
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            return base.StopAsync(cancellationToken);
        }
    }
}