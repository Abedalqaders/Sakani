using Application.Common.Interfaces.General;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Interfaces.Notification
{
    public interface ISystemNotificationRepository : IGenericRepository<Domain.Entities.Notification>
    {
        public Task ProcessDailyNotificationsAsync(DateTime now, DateTime contractExpiryThreshold, DateTime slaEscalationThreshold, CancellationToken cancellationToken);
    }
}
