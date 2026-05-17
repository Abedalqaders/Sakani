using Application.Interfaces;
using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Services;

public class SystemNotificationService : ISystemNotificationService
{
    private readonly IApplicationDbContext _dbContext;
    private readonly ILogger<SystemNotificationService> _logger;

    public SystemNotificationService(IApplicationDbContext dbContext, ILogger<SystemNotificationService> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task GenerateDailyNotificationsAsync(CancellationToken cancellationToken)
    {
        var now = DateTime.UtcNow;
        var notifications = new List<Notification>();

        using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            // 1. Update and Fetch Overdue Payments
            // أولاً: تحديث الدفعات التي تجاوزت تاريخ الاستحقاق من Pending إلى Overdue مباشرة في قاعدة البيانات
            await _dbContext.Payments
                .IgnoreQueryFilters()
                .Where(p => !p.IsDeleted && p.DueDate < now && p.PaymentStatus == PaymentStatus.Pending)
                .ExecuteUpdateAsync(s => s
                    .SetProperty(p => p.PaymentStatus, PaymentStatus.Overdue)
                    .SetProperty(p => p.UpdatedAt, now),
                    cancellationToken);

            // ثانياً: جلب الدفعات التي أصبحت Overdue ولم يتم توليد إشعار تأخير لها بعد
            var overduePayments = await _dbContext.Payments
                .IgnoreQueryFilters()
                .Where(p => !p.IsDeleted && p.PaymentStatus == PaymentStatus.Overdue && !p.IsOverdueNotificationSent)
                .Select(p => new
                {
                    p.Id,
                    p.TenantId,
                    RealUserId = p.Contract.Renter.UserId
                })
                .ToListAsync(cancellationToken);

            if (overduePayments.Count > 0)
            {
                var validPayments = overduePayments.Where(p => p.RealUserId.HasValue).ToList();

                notifications.AddRange(validPayments.Select(p => new Notification
                {
                    Id = Guid.NewGuid(),
                    Title = "Overdue Payment",
                    Message = "There is an overdue payment waiting to be settled.",
                    ReferenceId = p.Id,
                    TenantId = p.TenantId,
                    UserId = p.RealUserId!.Value,
                    Type = NotificationType.PaymentOverdue,
                    IsRead = false,
                    CreatedAt = now
                }));

                var overduePaymentIds = overduePayments.Select(x => x.Id).ToList();
                await _dbContext.Payments
                    .IgnoreQueryFilters()
                    .Where(p => overduePaymentIds.Contains(p.Id))
                    .ExecuteUpdateAsync(s => s.SetProperty(p => p.IsOverdueNotificationSent, true), cancellationToken);
            }

            // 2. Contract Expiration Reminder
            var thirtyDaysFromNow = now.AddDays(30);
            var expiringContracts = await _dbContext.Contracts
                .IgnoreQueryFilters()
                .Where(c => !c.IsDeleted && c.EndDate <= thirtyDaysFromNow && c.EndDate > now && c.ContractStatus == ContractStatus.Active && !c.IsExpirationReminderSent)
                .Select(c => new
                {
                    c.Id,
                    c.TenantId,
                    RealUserId = c.Renter.UserId
                })
                .ToListAsync(cancellationToken);

            if (expiringContracts.Count > 0)
            {
                var validContracts = expiringContracts.Where(c => c.RealUserId.HasValue).ToList();

                notifications.AddRange(validContracts.Select(c => new Notification
                {
                    Id = Guid.NewGuid(),
                    Title = "Contract Expiring Soon",
                    Message = $"Contract {c.Id} expires in less than 30 days.",
                    ReferenceId = c.Id,
                    TenantId = c.TenantId,
                    UserId = c.RealUserId!.Value,
                    Type = NotificationType.ContractRenewalReminder,
                    IsRead = false,
                    CreatedAt = now
                }));

                var expiringContractIds = expiringContracts.Select(x => x.Id).ToList();
                await _dbContext.Contracts
                    .IgnoreQueryFilters()
                    .Where(c => expiringContractIds.Contains(c.Id))
                    .ExecuteUpdateAsync(s => s.SetProperty(c => c.IsExpirationReminderSent, true), cancellationToken);
            }

            // 3. Overstaying Renters
            var overstayingContracts = await _dbContext.Contracts
                .IgnoreQueryFilters()
                .Where(c => !c.IsDeleted && c.EndDate < now && c.ContractStatus == ContractStatus.Active && !c.IsOverstayNotificationSent)
                .Select(c => new
                {
                    c.Id,
                    c.TenantId,
                    RealUserId = c.Renter.UserId
                })
                .ToListAsync(cancellationToken);

            if (overstayingContracts.Count > 0)
            {
                var validOverstays = overstayingContracts.Where(c => c.RealUserId.HasValue).ToList();

                notifications.AddRange(validOverstays.Select(c => new Notification
                {
                    Id = Guid.NewGuid(),
                    Title = "Contract Overstay",
                    Message = $"Contract {c.Id} has expired but is still active.",
                    ReferenceId = c.Id,
                    TenantId = c.TenantId,
                    UserId = c.RealUserId!.Value,
                    Type = NotificationType.ContractOverstayAlert,
                    IsRead = false,
                    CreatedAt = now
                }));

                var overstayingContractIds = overstayingContracts.Select(x => x.Id).ToList();
                await _dbContext.Contracts
                    .IgnoreQueryFilters()
                    .Where(c => overstayingContractIds.Contains(c.Id))
                    .ExecuteUpdateAsync(s => s.SetProperty(c => c.IsOverstayNotificationSent, true), cancellationToken);
            }

            // 4. SLA Escalation
            var slaLimit = now.AddHours(-48);
            var escalatedTickets = await _dbContext.MaintenanceTickets
                .IgnoreQueryFilters()
                .Where(t => !t.IsDeleted && t.TicketStatus == TicketStatus.Open && t.CreatedAt < slaLimit && !t.IsEscalationNotified)
                .Select(t => new
                {
                    t.Id,
                    t.TenantId,
                    ManagerUserId = _dbContext.Users
                        .IgnoreQueryFilters()
                        .Where(u => u.TenantId == t.TenantId && u.RoleId == 2 && !u.IsDeleted)
                        .Select(u => u.Id)
                        .FirstOrDefault()
                })
                .ToListAsync(cancellationToken);

            if (escalatedTickets.Count > 0)
            {
                var validTickets = escalatedTickets.Where(t => t.ManagerUserId != Guid.Empty).ToList();

                notifications.AddRange(validTickets.Select(t => new Notification
                {
                    Id = Guid.NewGuid(),
                    Title = "Maintenance SLA Escalation",
                    Message = $"Ticket {t.Id} has been open for more than 48 hours.",
                    ReferenceId = t.Id,
                    TenantId = t.TenantId,
                    UserId = t.ManagerUserId,
                    Type = NotificationType.MaintenanceEscalation,
                    IsRead = false,
                    CreatedAt = now
                }));

                var escalatedTicketIds = escalatedTickets.Select(x => x.Id).ToList();
                await _dbContext.MaintenanceTickets
                    .IgnoreQueryFilters()
                    .Where(t => escalatedTicketIds.Contains(t.Id))
                    .ExecuteUpdateAsync(s => s.SetProperty(t => t.IsEscalationNotified, true), cancellationToken);
            }

            // حفظ التغييرات النهائية في جدول الإشعارات
            if (notifications.Count > 0)
            {
                await _dbContext.Notifications.AddRangeAsync(notifications, cancellationToken);
                await _dbContext.SaveChangesAsync(cancellationToken);
            }

            await transaction.CommitAsync(cancellationToken);
            _logger.LogInformation("Generated {Count} daily system notifications successfully.", notifications.Count);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(cancellationToken);
            _logger.LogError(ex, "Failed to generate notifications. Transaction rolled back.");
            throw;
        }
    }
}