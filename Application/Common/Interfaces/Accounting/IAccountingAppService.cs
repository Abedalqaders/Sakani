using Application.Dto.Payment;


namespace Application.Common.Interfaces.Accounting
{
    public interface IAccountingService
    {
        Task<IReadOnlyList<PaymentResponse>> GetExpectedPaymentsAsync(DateTime startDate, DateTime endDate, CancellationToken ct);
        Task<IReadOnlyList<PaymentResponse>> GetOverduePaymentsAsync(CancellationToken ct);
        Task<DashboardStatsDto> GetDashboardStatsAsync(int month, int year, CancellationToken ct);
    }
}
