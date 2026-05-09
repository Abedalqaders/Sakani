using Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Api.Services
{
    public class ExpiryBackGroundService : BackgroundService
    {
        private readonly ILogger<ExpiryBackGroundService> _logger;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public ExpiryBackGroundService(ILogger<ExpiryBackGroundService> logger, IServiceScopeFactory serviceScopeFactory)
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
                        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                        await dbContext.Payments
                            .IgnoreQueryFilters()
                            .Where(p => !p.IsDeleted && p.DueDate < DateTime.UtcNow && p.PaymentStatus == Domain.Enums.PaymentStatus.Pending)
                            .ExecuteUpdateAsync(p => p
                                .SetProperty(x => x.PaymentStatus, Domain.Enums.PaymentStatus.Overdue)
                                .SetProperty(x => x.UpdatedAt, DateTime.UtcNow),
                                stoppingToken);
                        _logger.LogInformation("Expired payments updated successfully at {Time}", DateTime.UtcNow);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to update expired payments due to database connection issue.");
                }

                await Task.Delay(TimeSpan.FromHours(24), stoppingToken);
            }
        }
    }
}