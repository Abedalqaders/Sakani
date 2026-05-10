using Application.Common.Interfaces.General;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Common.Interfaces.Notification
{
    public interface INotificationRepository : IGenericRepository<Domain.Entities.Notification>
    {
        Task<IReadOnlyList<Domain.Entities.Notification>> GetUserNotificationsAsync(Guid userId, bool unreadOnly, CancellationToken cancellationToken = default);
        Task<Domain.Entities.Notification?> GetByIdAndUserAsync(Guid id, Guid userId, CancellationToken cancellationToken = default);
        Task<List<Domain.Entities.Notification>> GetUnreadByUserAsync(Guid userId, CancellationToken cancellationToken = default);
    }
}