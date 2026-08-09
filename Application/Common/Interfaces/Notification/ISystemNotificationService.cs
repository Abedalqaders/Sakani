using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Interfaces.Notification
{
    public interface ISystemNotificationService
    {
        public Task GenerateDailyNotificationsAsync(CancellationToken cancellationToken=default);
    }
}
