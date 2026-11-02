using Application.Dto.MaintenanceTicket;
using FluentValidation;

namespace Application.Validators.MaintenanceTicket
{
    public class TicketFilterDtoValidator : AbstractValidator<TicketFilterDto>
    {
        public TicketFilterDtoValidator()
        {
            RuleFor(x => x.Status)
                .IsInEnum().When(x => x.Status.HasValue)
                .WithMessage("Invalid ticket status.");

            RuleFor(x => x.SearchTerm)
                .MaximumLength(200).When(x => !string.IsNullOrWhiteSpace(x.SearchTerm))
                .WithMessage("Search term cannot exceed 200 characters.");

            RuleFor(x => x)
                .Must(x => !x.FromDate.HasValue || !x.ToDate.HasValue || x.FromDate <= x.ToDate)
                .WithMessage("FromDate must be less than or equal to ToDate.");
        }
    }
}