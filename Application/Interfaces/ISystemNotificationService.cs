namespace Application.Interfaces;

public interface ISystemNotificationService
{
    Task GenerateDailyNotificationsAsync(CancellationToken cancellationToken);
}