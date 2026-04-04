using Application.Dto.Payment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Interfaces
{
    public interface IAccountingService
    {
        Task<IReadOnlyList<PaymentResponse>> GetExpectedPaymentsAsync(DateTime startDate, DateTime endDate, CancellationToken ct);
        Task<IReadOnlyList<PaymentResponse>> GetOverduePaymentsAsync(CancellationToken ct);
        Task<DashboardStatsDto> GetDashboardStatsAsync(int month, int year, CancellationToken ct);
    }
}
