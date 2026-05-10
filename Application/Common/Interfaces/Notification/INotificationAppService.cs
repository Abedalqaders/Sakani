using Application.Dto.Notification;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Common.Interfaces.Notification
{
    public interface INotificationAppService
    {
        Task<IReadOnlyList<NotificationResponseDto>> GetMyNotificationsAsync(bool unreadOnly, CancellationToken kt = default);
        Task MarkAsReadAsync(Guid id, CancellationToken kt = default);
        Task MarkAllAsReadAsync(CancellationToken kt = default);
    }
}