using Application.Dto.Payment;


namespace Application.Common.Interfaces.Accounting
{
    public interface IAccountingService
    {
        Task<IReadOnlyList<PaymentResponse>> GetExpectedPaymentsAsync(DateTime startDate, DateTime endDate, CancellationToken ct);
        Task<IReadOnlyList<PaymentResponse>> GetOverduePaymentsAsync(CancellationToken ct);
        Task<DashboardStatsDto> GetDashboardStatsAsync(int month, int year, CancellationToken ct);
        Task<IReadOnlyList<PaymentHistoryResponseDto>> GetMyPaymentHistoryAsync(CancellationToken ct);
        Task<IReadOnlyList<PendingPaymentResponseDto>> GetMyPendingPaymentsAsync(CancellationToken ct);
        Task<PaymentDetailsDto> GetPaymentDetailsAsync(Guid paymentId, CancellationToken ct);
    }
}
