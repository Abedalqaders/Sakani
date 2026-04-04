using Application.Common.Interfaces;
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
    }
}
