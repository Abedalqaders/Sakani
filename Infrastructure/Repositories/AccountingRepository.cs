using Application.Common.Interfaces.Accounting;
using Application.Dto.Payment;
using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class AccountingRepository : IAccountingRepository
    {
        private readonly ApplicationDbContext _context;

        public AccountingRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IReadOnlyList<PaymentResponse>> GetAllExpctedPayment(DateTime startDate, DateTime endDate, CancellationToken ct)
        {
            return await _context.Set<Payment>()
                .AsNoTracking()
                .Where(p => p.DueDate >= startDate && p.DueDate <= endDate && p.PaymentStatus == PaymentStatus.Pending)
                .Select(p => new PaymentResponse
                {
                    PaymentId = p.Id,
                    Amount = p.Amount,
                    DueDate = p.DueDate,
                    PaymentStatus = p.PaymentStatus,
                    ContractId = p.ContractId,
                    RenterName = p.Contract.Renter.FirstName + " " + p.Contract.Renter.LastName,
                    RenterPhoneNumber = p.Contract.Renter.PhoneNumber,
                    PropertyName = p.Contract.Unit.Property.Name,
                    UnitNo = p.Contract.Unit.UnitNo
                })
                .ToListAsync(ct);
        }

        public async Task<IReadOnlyList<PaymentResponse>> GetAllOverDuePayment(CancellationToken ct)
        {
            return await _context.Set<Payment>()
                .AsNoTracking()
                .Where(p => (p.DueDate < DateTime.UtcNow && p.PaymentStatus == PaymentStatus.Pending) || p.PaymentStatus == PaymentStatus.Overdue)
                .Select(p => new PaymentResponse
                {
                    PaymentId = p.Id,
                    Amount = p.Amount,
                    DueDate = p.DueDate,
                    PaymentStatus = p.PaymentStatus,
                    ContractId = p.ContractId,
                    RenterName = p.Contract.Renter.FirstName + " " + p.Contract.Renter.LastName,
                    RenterPhoneNumber = p.Contract.Renter.PhoneNumber,
                    PropertyName = p.Contract.Unit.Property.Name,
                    UnitNo = p.Contract.Unit.UnitNo
                }).ToListAsync(ct);
        }

        public async Task<decimal> GetExpectedRentAmountForMonth(int month, int year, CancellationToken ct)
        {
            var startDate = new DateTime(year, month, 1, 0, 0, 0, DateTimeKind.Utc);
            var endDate = startDate.AddMonths(1).AddTicks(-1);

            return await _context.Set<Payment>()
                .AsNoTracking()
                .Where(p => p.DueDate >= startDate && p.DueDate <= endDate &&
                           (p.PaymentStatus == PaymentStatus.Pending || p.PaymentStatus == PaymentStatus.Paid || p.PaymentStatus == PaymentStatus.Overdue))
                .SumAsync(p => (decimal?)p.Amount, ct) ?? 0m;
        }

        public async Task<decimal> GetTotalCollectedMonth(int month, int year, CancellationToken ct)
        {
            var startDate = new DateTime(year, month, 1, 0, 0, 0, DateTimeKind.Utc);
            var endDate = startDate.AddMonths(1).AddTicks(-1);

            return await _context.Set<Payment>()
                .AsNoTracking()
                .Where(p => p.DueDate >= startDate && p.DueDate <= endDate && p.PaymentStatus == PaymentStatus.Paid)
                .SumAsync(p => (decimal?)p.Amount, ct) ?? 0m;
        }

        public async Task<decimal> GetOccupancyRateAsync(CancellationToken ct)
        {
            var stats = await _context.Set<Unit>()
                .AsNoTracking()
                .GroupBy(_ => 1)
                .Select(g => new
                {
                    Total = g.Count(),
                    Rented = g.Count(u => u.UnitStatus == UnitStatus.Rented)
                })
                .FirstOrDefaultAsync(ct);

            if (stats == null || stats.Total == 0) return 0m;

            return Math.Round((decimal)stats.Rented / stats.Total * 100, 2);
        }
        public async Task<IReadOnlyList<PaymentHistoryResponseDto>> GetPaymentHistoryForRenterAsync(Guid renterId, CancellationToken ct)
        {
            return await _context.Set<Payment>()
                .AsNoTracking() // لأنها عملية قراءة فقط
                .Where(p => p.Contract.RenterId == renterId && p.PaymentStatus == PaymentStatus.Paid)
                .OrderByDescending(p => p.ActualPaymentDate) // أحدث الدفعات أولاً
                .Select(p => new PaymentHistoryResponseDto
                {
                    PaymentId = p.Id,
                    Amount = p.Amount,
                    DueDate = p.DueDate,
                    // نستخدم القيمة الفعلية، وإذا كانت null (رغم إنها المفروض ما تكون لدفعة مدفوعة) بنحط تاريخ اليوم كحماية
                    PaymentDate = p.ActualPaymentDate ?? DateTime.UtcNow,
                    TransactionId = p.TransactionId ?? "N/A",
                    PropertyName = p.Contract.Unit.Property.Name,
                    UnitNo = p.Contract.Unit.UnitNo
                })
                .ToListAsync(ct);
        }
        public async Task<IReadOnlyList<PendingPaymentResponseDto>> GetPendingPaymentsForRenterAsync(Guid renterId, CancellationToken ct)
        {
            return await _context.Set<Payment>()
                .AsNoTracking()
                .Where(p => p.Contract.RenterId == renterId &&
                           (p.PaymentStatus == PaymentStatus.Pending || p.PaymentStatus == PaymentStatus.Overdue))
                .OrderBy(p => p.DueDate) // الأقدم (المستحق أولاً) بيطلع فوق
                .Select(p => new PendingPaymentResponseDto
                {
                    PaymentId = p.Id,
                    Amount = p.Amount,
                    DueDate = p.DueDate,
                    PropertyName = p.Contract.Unit.Property.Name,
                    UnitNo = p.Contract.Unit.UnitNo
                })
                .ToListAsync(ct);
        }
        public async Task<PaymentDetailsDto?> GetPaymentDetailsForRenterAsync(Guid paymentId, Guid renterId, CancellationToken ct)
        {
            return await _context.Set<Payment>()
                .AsNoTracking()
                .Where(p => p.Id == paymentId && p.Contract.RenterId == renterId)
                .Select(p => new PaymentDetailsDto
                {
                    PaymentId = p.Id,
                    Amount = p.Amount,
                    DueDate = p.DueDate,
                    PropertyName = p.Contract.Unit.Property.Name,
                    UnitNo = p.Contract.Unit.UnitNo,
                    ContractStartDate = p.Contract.StartDate,
                    ContractEndDate = p.Contract.EndDate
                })
                .FirstOrDefaultAsync(ct);
        }
    }
}
