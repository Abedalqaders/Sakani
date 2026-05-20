using Application.Dto.MaintenanceTicket;
using FluentValidation;

namespace Application.Validators.MaintenanceTicket
{
    public class CreateTicketDtoValidator : AbstractValidator<CreateTicketDto>
    {
        public CreateTicketDtoValidator()
        {
          

            RuleFor(x => x.Subject)
                .NotEmpty().WithMessage("Subject is required.")
                .MaximumLength(200).WithMessage("Subject cannot exceed 200 characters.");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Description is required.")
                .MaximumLength(2000).WithMessage("Description cannot exceed 2000 characters.");
        }
    }
}