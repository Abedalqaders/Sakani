using Application.Common.Interfaces.Payment;
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
    public class PaymentRepository:GenericRepository<Payment>, IPaymentRepository
    {
        public PaymentRepository(ApplicationDbContext context):base(context)
        {
        }
        public async Task<Payment?> GetPaymentWithContractAsync(Guid paymentId, Guid renterId, CancellationToken ct)
        {
            return await _context.Set<Payment>()
        .FirstOrDefaultAsync(p => p.Id == paymentId && p.Contract.RenterId == renterId, ct);
        }
        public async Task<bool> CanPayInstallmentAsync(Payment payment, CancellationToken ct)
        {
            if (payment == null)
                return false;
            bool hasEarlierUnpaid = await _context.Set<Payment>()
        .AnyAsync(p =>
          (p.PaymentStatus==Domain.Enums.PaymentStatus.Pending|| p.PaymentStatus==PaymentStatus.Overdue) &&p.ContractId == payment.ContractId &&
            p.DueDate < payment.DueDate, ct);
            return !hasEarlierUnpaid;

        }
        public async Task<bool> CancelPaymentForContract(Guid ContractId, CancellationToken ct)
        {
            var payments = await _context.Set<Payment>()
                .Where(p => p.ContractId == ContractId && p.PaymentStatus == PaymentStatus.Pending && p.DueDate>DateTime.UtcNow)
                .ToListAsync(ct);
            if (payments.Count == 0)
            {
                return false; 
            }
            foreach (var payment in payments)
            {
                payment.PaymentStatus = PaymentStatus.Cancelled;
            }
            return true;
        }
  
    }
}
