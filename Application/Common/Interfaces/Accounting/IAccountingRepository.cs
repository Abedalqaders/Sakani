using Application.Dto.Payment;


namespace Application.Common.Interfaces.Accounting
{
    public interface IAccountingRepository
    {
        Task<IReadOnlyList<PaymentResponse>> GetAllExpctedPayment(DateTime startDate, DateTime endDate, CancellationToken ct);
        Task<IReadOnlyList<PaymentResponse>> GetAllOverDuePayment(CancellationToken ct);
        Task<decimal> GetExpectedRentAmountForMonth(int month, int year, CancellationToken ct);
        Task<decimal> GetTotalCollectedMonth(int month, int year, CancellationToken ct);
        Task<decimal> GetOccupancyRateAsync(CancellationToken ct);
    }
}
