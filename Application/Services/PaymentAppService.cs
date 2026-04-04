using Application.Common.Interfaces;
using Application.Dto.Payment;
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
        public PaymentAppService(IPaymentRepository paymentRepo)
        {
            _paymentRepo = paymentRepo;
        }
        
    }
}
