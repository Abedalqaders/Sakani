using Application.Common.Interfaces.Accounting;
using Application.Common.Interfaces.General;
using Application.Common.Interfaces.Payment;
using Application.Common.Interfaces.User;
using Application.Dto.Payment;
using Domain.Entities;
using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class PaymentAppService : IPaymentAppService
    {
        private readonly IPaymentRepository _paymentRepo;
        private readonly ICurrentUserService _currentUserService;
        private readonly IUnitOfWork _unitOfWork;
        public PaymentAppService(IPaymentRepository paymentRepo)
        {
            _paymentRepo = paymentRepo;
        }
        public async Task<string> PayWithCreditCardSimulatedAsync(PaymentSimulationDto dto, CancellationToken ct)
        {
            var renterId = _currentUserService.RenterId.GetValueOrDefault();

            // السطر هذا بجيب الدفعة فقط إذا كانت موجودة وتابعة لهذا المستأجر
            var payment = await _paymentRepo.GetPaymentWithContractAsync(dto.PaymentId, renterId, ct);

            if (payment == null)
                throw new KeyNotFoundException("Payment not found or access denied.");

            if (payment.PaymentStatus == PaymentStatus.Paid)
                throw new InvalidOperationException("This payment has already been paid.");

            // اللوجيك تبع المحاكاة
            await Task.Delay(3000, ct);
            var transactionId = "TRX-" + Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper();

            payment.PaymentStatus = PaymentStatus.Paid;
            payment.TransactionId = transactionId;
            payment.ActualPaymentDate = DateTime.UtcNow;

            _paymentRepo.Update(payment);
            await _unitOfWork.SaveChangesAsync(ct);

            return transactionId;
        }


    }
}
