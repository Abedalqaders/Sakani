using Application.Dto.Renter;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Validators.Renter
{
    public class CreateRenterDtoValidator : AbstractValidator<CreateRenterDto>
    {
        public CreateRenterDtoValidator()
        {
            RuleFor(x => x.NationalId)
                .NotEmpty().WithMessage("National ID is required.")
                .Length(10).WithMessage("National ID must be exactly 10 digits.");

            RuleFor(x => x.PhoneNumber)
                .NotEmpty().WithMessage("Phone number is required.")
                .Matches(@"^\d{10}$").WithMessage("Invalid phone number format.");

            RuleFor(x => x.Email)
                .NotEmpty().EmailAddress().WithMessage("A valid email is required for the renter's account.");
        }
    }
}
