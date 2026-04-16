using Application.Common.Interfaces.Accounting;
using Application.Common.Interfaces.User;
using Application.Dto.Payment;


namespace Application.Services
{
    public class AccountingService : IAccountingService
    {
        private readonly IAccountingRepository _accountingRepo;
        private readonly ICurrentUserService _currentUserService;

        public AccountingService(IAccountingRepository accountingRepo, ICurrentUserService currentUserService   )
        {
            _accountingRepo = accountingRepo;
            _currentUserService = currentUserService;
        }
        public async Task<IReadOnlyList<PaymentHistoryResponseDto>> GetMyPaymentHistoryAsync(CancellationToken ct)
        {
            // بنجيب الـ ID تبع المستأجر من الـ Token
            var renterId = _currentUserService.RenterId.GetValueOrDefault();

            return await _accountingRepo.GetPaymentHistoryForRenterAsync(renterId, ct);
        }
        public async Task<IReadOnlyList<PendingPaymentResponseDto>> GetMyPendingPaymentsAsync(CancellationToken ct)
        {
            var renterId = _currentUserService.RenterId.GetValueOrDefault();
            return await _accountingRepo.GetPendingPaymentsForRenterAsync(renterId, ct);
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
        public async Task<PaymentDetailsDto> GetPaymentDetailsAsync(Guid paymentId, CancellationToken ct)
        {
            var renterId = _currentUserService.RenterId.GetValueOrDefault();

            var details = await _accountingRepo.GetPaymentDetailsForRenterAsync(paymentId, renterId, ct);

            if (details == null)
                throw new KeyNotFoundException("Payment details not found or access denied.");

            return details;
        }
    }
}