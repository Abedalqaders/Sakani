using Application.Interfaces;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

namespace Api.Services;

public class NotificationBackgroundService : BackgroundService
{
    private readonly ILogger<NotificationBackgroundService> _logger;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public NotificationBackgroundService(
        ILogger<NotificationBackgroundService> logger, 
        IServiceScopeFactory serviceScopeFactory)
    {
        _logger = logger;
        _serviceScopeFactory = serviceScopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation($"{nameof(NotificationBackgroundService)} is starting.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                _logger.LogInformation("Starting daily notification generation task...");

                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var notificationService = scope.ServiceProvider.GetRequiredService<ISystemNotificationService>();
                    await notificationService.GenerateDailyNotificationsAsync(stoppingToken);
                }

                _logger.LogInformation("Successfully completed daily notification generation.");
            }
            catch (Exception ex)
            {
                // Catching exception prevents the Host from crashing due to transient DB failures
                _logger.LogError(ex, "An error occurred while running the daily notification generation job.");
            }

            // Delay for 24 hours before running the next iteration
            await Task.Delay(TimeSpan.FromHours(24), stoppingToken);
        }

        _logger.LogInformation($"{nameof(NotificationBackgroundService)} is stopping due to app cancellation.");
    }
}