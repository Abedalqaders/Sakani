using Application.Dto.Payment;
using Domain.Enums;


namespace Application.Common.Interfaces.Accounting
{
    public interface IAccountingRepository
    {
        Task<IReadOnlyList<PaymentResponse>> GetAllExpctedPayment(DateTime startDate, DateTime endDate, CancellationToken ct);
        Task<IReadOnlyList<PaymentResponse>> GetAllOverDuePayment(CancellationToken ct);
        Task<decimal> GetExpectedRentAmountForMonth(int month, int year, CancellationToken ct);
        Task<decimal> GetTotalCollectedMonth(int month, int year, CancellationToken ct);
        Task<decimal> GetOccupancyRateAsync(CancellationToken ct);
        Task<IReadOnlyList<PaymentHistoryResponseDto>> GetPaymentHistoryForRenterAsync(Guid renterId, PaymentFilterType filter, CancellationToken ct);
        Task<IReadOnlyList<PendingPaymentResponseDto>> GetPendingPaymentsForRenterAsync(Guid renterId, CancellationToken ct);
        Task<PaymentDetailsDto?> GetPaymentDetailsForRenterAsync(Guid paymentId, Guid renterId, CancellationToken ct);
        Task<decimal> GetExpensesForRangeAsync(DateTime startDate, DateTime endDate, CancellationToken ct);

    }
}
