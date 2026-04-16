using Application.Dto.Payment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Common.Interfaces.General;

namespace Application.Common.Interfaces.Payment
{
    public interface IPaymentRepository: IGenericRepository<Domain.Entities.Payment>
    {
        Task<bool> CancelPaymentForContract(Guid ContractId, CancellationToken ct);
        Task<Domain.Entities.Payment?> GetPaymentWithContractAsync(Guid paymentId, Guid renterId,CancellationToken ct);
    }
}
