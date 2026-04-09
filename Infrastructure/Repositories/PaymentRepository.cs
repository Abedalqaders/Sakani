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
