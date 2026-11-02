using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Application.Common.Interfaces.Notification;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Api.Services
{
    public class NotificationBackgroundService : BackgroundService
    {
        private readonly ILogger<NotificationBackgroundService> _logger;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public NotificationBackgroundService(ILogger<NotificationBackgroundService> logger, IServiceScopeFactory serviceScopeFactory)
        {
            _logger = logger;
            _serviceScopeFactory = serviceScopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using (var scope = _serviceScopeFactory.CreateScope())
                    {
                        var notificationService = scope.ServiceProvider.GetRequiredService<ISystemNotificationService>();
                        await notificationService.GenerateDailyNotificationsAsync(stoppingToken);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while running the daily notification generation job.");
                }

                // حساب الوقت المتبقي حتى منتصف الليل بالتوقيت العالمي (UTC) لتشغيل العملية بدقة تامة يوميا
                var now = DateTime.UtcNow;
                var nextRunTime = now.Date.AddDays(1); // اليوم التالي الساعة 00:00:00
                var delay = nextRunTime - now;

                await Task.Delay(delay, stoppingToken);
            }
        }
    }
}