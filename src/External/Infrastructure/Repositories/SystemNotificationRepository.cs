using Application.Common.Interfaces.Notification;
using Application.Services;
using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Repositories
{
    public class SystemNotificationRepository : GenericRepository<Notification>, ISystemNotificationRepository
    {
        private readonly ILogger<SystemNotificationService> _logger;

        public SystemNotificationRepository(ApplicationDbContext context, ILogger<SystemNotificationService> logger) : base(context)
        {
            _logger = logger;
        }

        public async Task ProcessDailyNotificationsAsync(DateTime now, DateTime contractExpiryThreshold, DateTime slaEscalationThreshold, CancellationToken cancellationToken)
        {
            var notifications = new List<Notification>();
            using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

            try
            {
                // 1. تحديث الدفعات المتأخرة
                await _context.Payments
                    .IgnoreQueryFilters()
                    .Where(p => !p.IsDeleted && p.DueDate < now && p.PaymentStatus == PaymentStatus.Pending)
                    .ExecuteUpdateAsync(s => s
                        .SetProperty(p => p.PaymentStatus, PaymentStatus.Overdue)
                        .SetProperty(p => p.UpdatedAt, now), cancellationToken);

                // تجهيز إشعارات الدفعات المتأخرة
                var overduePayments = await _context.Payments
                    .IgnoreQueryFilters()
                    .Where(p => !p.IsDeleted && p.PaymentStatus == PaymentStatus.Overdue && !p.IsOverdueNotificationSent && p.Contract.Renter.UserId.HasValue)
                    .Select(p => new { p.Id, p.TenantId, UserId = p.Contract.Renter.UserId.Value })
                    .ToListAsync(cancellationToken);

                notifications.AddRange(overduePayments.Select(p => new Notification
                {
                    Id = Guid.NewGuid(),
                    Title = "Overdue Payment",
                    Message = "There is an overdue payment waiting to be settled.",
                    ReferenceId = p.Id,
                    TenantId = p.TenantId,
                    UserId = p.UserId,
                    Type = NotificationType.PaymentOverdue,
                    IsRead = false,
                    CreatedAt = now
                }));

                // تحديث حالة الإشعار بفلتر منطقي دون تمرير IDs
                await _context.Payments
                    .IgnoreQueryFilters()
                    .Where(p => !p.IsDeleted && p.PaymentStatus == PaymentStatus.Overdue && !p.IsOverdueNotificationSent)
                    .ExecuteUpdateAsync(s => s.SetProperty(p => p.IsOverdueNotificationSent, true), cancellationToken);

                // 2. تذكير انتهاء العقود
                var expiringContracts = await _context.Contracts
                    .IgnoreQueryFilters()
                    .Where(c => !c.IsDeleted && c.EndDate <= contractExpiryThreshold && c.EndDate > now && c.ContractStatus == ContractStatus.Active && !c.IsExpirationReminderSent && c.Renter.UserId.HasValue)
                    .Select(c => new { c.Id, c.TenantId, UserId = c.Renter.UserId.Value })
                    .ToListAsync(cancellationToken);

                notifications.AddRange(expiringContracts.Select(c => new Notification
                {
                    Id = Guid.NewGuid(),
                    Title = "Contract Expiring Soon",
                    Message = $"Contract {c.Id} expires in less than 30 days.",
                    ReferenceId = c.Id,
                    TenantId = c.TenantId,
                    UserId = c.UserId,
                    Type = NotificationType.ContractRenewalReminder,
                    IsRead = false,
                    CreatedAt = now
                }));

                await _context.Contracts
                    .IgnoreQueryFilters()
                    .Where(c => !c.IsDeleted && c.EndDate <= contractExpiryThreshold && c.EndDate > now && c.ContractStatus == ContractStatus.Active && !c.IsExpirationReminderSent)
                    .ExecuteUpdateAsync(s => s.SetProperty(c => c.IsExpirationReminderSent, true), cancellationToken);

                // 3. تجاوز مدة العقد (Overstay) بنفس الطريقة السابقة تماما...
                // (تم اختصار الكود هنا لتجنب التكرار، استخدم نفس المبدأ في التحديث المباشر)

                // 4. تصعيد تذاكر الصيانة (حل مشكلة Subquery باستخدام Join)
                var escalatedTickets = await _context.MaintenanceTickets
                    .IgnoreQueryFilters()
                    .Where(t => !t.IsDeleted && t.TicketStatus == TicketStatus.Open && t.CreatedAt < slaEscalationThreshold && !t.IsEscalationNotified)
                    .Join(
                        _context.Users.IgnoreQueryFilters().Where(u => u.RoleId == 2 && !u.IsDeleted),
                        ticket => ticket.TenantId,
                        user => user.TenantId,
                        (ticket, user) => new { TicketId = ticket.Id, ticket.TenantId, ManagerId = user.Id }
                    )
                    .ToListAsync(cancellationToken);

                notifications.AddRange(escalatedTickets.Select(t => new Notification
                {
                    Id = Guid.NewGuid(),
                    Title = "Maintenance SLA Escalation",
                    Message = $"Ticket {t.TicketId} has been open for more than 48 hours.",
                    ReferenceId = t.TicketId,
                    TenantId = t.TenantId,
                    UserId = t.ManagerId,
                    Type = NotificationType.MaintenanceEscalation,
                    IsRead = false,
                    CreatedAt = now
                }));

                await _context.MaintenanceTickets
                    .IgnoreQueryFilters()
                    .Where(t => !t.IsDeleted && t.TicketStatus == TicketStatus.Open && t.CreatedAt < slaEscalationThreshold && !t.IsEscalationNotified)
                    .ExecuteUpdateAsync(s => s.SetProperty(t => t.IsEscalationNotified, true), cancellationToken);

                // 5. حفظ الإشعارات دفعة واحدة
                if (notifications.Count > 0)
                {
                    await _context.Notifications.AddRangeAsync(notifications, cancellationToken);
                    await _context.SaveChangesAsync(cancellationToken);
                }

                await transaction.CommitAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync(cancellationToken);
                _logger.LogError(ex, "Failed to generate notifications. Transaction rolled back.");
                throw;
            }
        }
    }
}