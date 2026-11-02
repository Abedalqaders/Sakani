using Application.Dto.MaintenanceTicket;
using FluentValidation;

namespace Application.Validators.MaintenanceTicket
{
    public class UpdateTicketStatusDtoValidator : AbstractValidator<UpdateTicketStatusDto>
    {
        public UpdateTicketStatusDtoValidator()
        {
            RuleFor(x => x.TicketId)
                .NotEmpty().WithMessage("Ticket ID is required.");

            RuleFor(x => x.NewStatus)
                .IsInEnum().WithMessage("Invalid ticket status.");
        }
    }
}