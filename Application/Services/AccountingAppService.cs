using Application.Common.Interfaces;
using Application.Dto.Payment;


namespace Application.Services
{
    public class AccountingService : IAccountingService
    {
        private readonly IAccountingRepository _accountingRepo;

        public AccountingService(IAccountingRepository accountingRepo)
        {
            _accountingRepo = accountingRepo;
        }

        public async Task<IReadOnlyList<PaymentResponse>> GetExpectedPaymentsAsync(DateTime startDate, DateTime endDate, CancellationToken ct)
        {
            if (startDate == default || endDate == default)
            {
                startDate = DateTime.UtcNow.Date;
                endDate = DateTime.UtcNow.AddMonths(1).Date;
            }

            if (startDate > endDate)
            {
                throw new ArgumentException("Start date must be earlier than end date.");
            }

            return await _accountingRepo.GetAllExpctedPayment(startDate, endDate, ct);
        }

        public async Task<IReadOnlyList<PaymentResponse>> GetOverduePaymentsAsync(CancellationToken ct)
        {
            return await _accountingRepo.GetAllOverDuePayment(ct);
        }

        public async Task<DashboardStatsDto> GetDashboardStatsAsync(int month, int year, CancellationToken ct)
        {
            var expected = await _accountingRepo.GetExpectedRentAmountForMonth(month, year, ct);
            var collected = await _accountingRepo.GetTotalCollectedMonth(month, year, ct);
            var occupancy = await _accountingRepo.GetOccupancyRateAsync(ct);

            return new DashboardStatsDto
            {
                TotalExpectedMonth = expected,
                TotalCollectedMonth = collected,
                OccupancyRate = occupancy
            };
        }
    }
}