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
            // 1. Overdue Payments
            var overduePayments = await _dbContext.Payments
                .IgnoreQueryFilters()
                .Where(p => !p.IsDeleted && p.DueDate < now && p.PaymentStatus == PaymentStatus.Pending && !p.IsOverdueNotificationSent)
                .Select(p => new
                {
                    p.Id,
                    p.TenantId,
                    // استخراج معرف المستخدم الحقيقي من جدول المستأجر
                    RealUserId = p.Contract.Renter.UserId
                })
                .ToListAsync(cancellationToken);

            if (overduePayments.Count > 0)
            {
                notifications.AddRange(overduePayments.Select(p => new Notification
                {
                    Title = "Overdue Payment",
                    Message = "There is an overdue payment waiting to be settled.",
                    ReferenceId = p.Id,
                    TenantId = p.TenantId,
                    UserId = p.RealUserId ?? Guid.Empty,
                    Type = NotificationType.PaymentOverdue,
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
                    // استخراج معرف المستخدم الحقيقي
                    RealUserId = c.Renter.UserId
                })
                .ToListAsync(cancellationToken);

            if (expiringContracts.Count > 0)
            {
                notifications.AddRange(expiringContracts.Select(c => new Notification
                {
                    Title = "Contract Expiring Soon",
                    Message = $"Contract {c.Id} expires in less than 30 days.",
                    ReferenceId = c.Id,
                    TenantId = c.TenantId,
                    UserId = c.RealUserId ?? Guid.Empty,
                    Type = NotificationType.ContractRenewalReminder,
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
                notifications.AddRange(overstayingContracts.Select(c => new Notification
                {
                    Title = "Contract Overstay",
                    Message = $"Contract {c.Id} has expired but is still active.",
                    ReferenceId = c.Id,
                    TenantId = c.TenantId,
                    UserId = c.RealUserId ?? Guid.Empty,
                    Type = NotificationType.ContractOverstayAlert,
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
                .Select(t => new { t.Id, t.TenantId })
                .ToListAsync(cancellationToken);

            if (escalatedTickets.Count > 0)
            {
                notifications.AddRange(escalatedTickets.Select(t => new Notification
                {
                    Title = "Maintenance SLA Escalation",
                    Message = $"Ticket {t.Id} has been open for more than 48 hours.",
                    ReferenceId = t.Id,
                    TenantId = t.TenantId,
                    UserId = Guid.Empty, // ما زالت تحتاج إلى تعديل لتوجيهها لمدير النظام
                    Type = NotificationType.MaintenanceEscalation,
                    CreatedAt = now
                }));

                var escalatedTicketIds = escalatedTickets.Select(x => x.Id).ToList();
                await _dbContext.MaintenanceTickets
                    .IgnoreQueryFilters()
                    .Where(t => escalatedTicketIds.Contains(t.Id))
                    .ExecuteUpdateAsync(s => s.SetProperty(t => t.IsEscalationNotified, true), cancellationToken);
            }

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