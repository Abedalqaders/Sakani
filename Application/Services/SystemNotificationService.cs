using Application.Common.Interfaces.Notification;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Services
{
    public class SystemNotificationService : ISystemNotificationService
    {
        private readonly ISystemNotificationRepository _systemNotificationRepository;

        public SystemNotificationService(ISystemNotificationRepository systemNotificationRepository)
        {
            _systemNotificationRepository = systemNotificationRepository;
        }

        public async Task GenerateDailyNotificationsAsync(CancellationToken cancellationToken = default)
        {
            var now = DateTime.UtcNow;


            var contractExpiryThreshold = now.AddDays(30);
            var slaEscalationThreshold = now.AddHours(-48);

            await _systemNotificationRepository.ProcessDailyNotificationsAsync(
                now,
                contractExpiryThreshold,
                slaEscalationThreshold,
                cancellationToken);
        }
    }
}