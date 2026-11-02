using FluentValidation;
using Application.Dto.Contract;
using Domain.Enums;

namespace Application.Validators.Contract
{
    public class CreateContractDtoValidator : AbstractValidator<CreateContractDto>
    {
        public CreateContractDtoValidator()
        {

            RuleFor(x => x.StartDate)
                .NotEmpty().WithMessage("Start date is required.")
                .GreaterThanOrEqualTo(DateTime.UtcNow.Date).WithMessage("Contract cannot start in the past.");

            RuleFor(x => x.EndDate)
                .NotEmpty().WithMessage("End date is required.")
                .GreaterThan(x => x.StartDate).WithMessage("End date must be after the start date.");

            RuleFor(x => x.EndDate)
                .Must((model, endDate) => endDate <= model.StartDate.AddYears(5))
                .WithMessage("The maximum contract duration is 5 years.");

            RuleFor(x => x.RentAmount)
                .GreaterThan(0).WithMessage("Rent amount must be a positive value.");


            RuleFor(x => x.UnitId)
                .NotEmpty().WithMessage("A Unit must be selected.");

            RuleFor(x => x.RenterId)
                .NotEmpty().WithMessage("A Renter must be selected.");

            // 4. Enum Validation
            RuleFor(x => (int)x.ContractStatus).Must(val => Enum.IsDefined(typeof(ContractStatus), (byte)val))
           .WithMessage("Invalid Contract Status status.");

            RuleFor(x => (int)x.PaymentFreq).Must(val => Enum.IsDefined(typeof(PaymentFrequency), (byte)val))
           .WithMessage("Invalid Payment Freq status.");

            // 5. Advanced Business Rule: Duration Compatibility
            // Ensures the total months can be divided evenly by the payment interval.
            RuleFor(x => x)
                .Must(HaveCompatibleDuration)
                .WithMessage("The contract duration must be divisible by the payment frequency (e.g., a 10-month contract cannot be paid quarterly).");
        }

        private bool HaveCompatibleDuration(CreateContractDto dto)
        {
            if (dto.StartDate >= dto.EndDate) return false;

            // Calculate total months between dates
            int totalMonths = ((dto.EndDate.Year - dto.StartDate.Year) * 12) + dto.EndDate.Month - dto.StartDate.Month;

            // PaymentFreq as byte/int represents months (Monthly=1, Quarterly=3, etc.)
            int monthsPerInstallment = (int)dto.PaymentFreq;

            // Duration must be divisible by the interval with no remainder
            return totalMonths > 0 && totalMonths % monthsPerInstallment == 0;
        }
    }
}