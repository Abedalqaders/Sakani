using Application.Common.Interfaces.Notification;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class NotificationRepository : GenericRepository<Domain.Entities.Notification>, INotificationRepository
    {
        private readonly ApplicationDbContext _context;

        public NotificationRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IReadOnlyList<Domain.Entities.Notification>> GetUserNotificationsAsync(Guid userId, bool unreadOnly, CancellationToken cancellationToken = default)
        {
            var query = _context.Set<Domain.Entities.Notification>()
                .Where(n => n.UserId == userId);

            if (unreadOnly)
            {
                query = query.Where(n => !n.IsRead);
            }

            return await query
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync(cancellationToken);
        }

        public async Task<Domain.Entities.Notification?> GetByIdAndUserAsync(Guid id, Guid userId, CancellationToken cancellationToken = default)
        {
            return await _context.Set<Domain.Entities.Notification>()
                .FirstOrDefaultAsync(n => n.Id == id && n.UserId == userId, cancellationToken);
        }

        public async Task<List<Domain.Entities.Notification>> GetUnreadByUserAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            return await _context.Set<Domain.Entities.Notification>()
                .Where(n => n.UserId == userId && !n.IsRead)
                .ToListAsync(cancellationToken);
        }
    }
}