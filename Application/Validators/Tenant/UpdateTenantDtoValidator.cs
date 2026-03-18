using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using Application.Dto.Tenant;

namespace Application.Validators.Tenant
{
    public class UpdateTenantDtoValidator: AbstractValidator<UpdateTenantDto>
    {
        public UpdateTenantDtoValidator()
        {
            RuleFor(x => x.Name)
                     .NotEmpty().WithMessage("Name is required.")
                     .MinimumLength(2).WithMessage("Name must be at least 2 characters long.")
                     .MaximumLength(150).WithMessage("Name cannot exceed 150 characters.");

            RuleFor(x => x.AddressCity)
                .NotEmpty().WithMessage("City is required.")
                .MaximumLength(100).WithMessage("City name is too long.");

            RuleFor(x => x.AddressStreet)
                .NotEmpty().WithMessage("Street address is required.")
                .MaximumLength(200).WithMessage("Street address is too long.");

            RuleFor(x => x.AddressRegion)
                .NotEmpty().WithMessage("Region is required.")
                .MaximumLength(100).WithMessage("Region name is too long.");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email format.")
                .MaximumLength(256).WithMessage("Email is too long.");

            RuleFor(x => x.PhoneNumber)
                .NotEmpty().WithMessage("Phone number is required.")
                .Matches(@"^\+?[0-9\s\-]{7,20}$").WithMessage("Invalid phone number format.");

            RuleFor(x => x.Status)
                .IsInEnum().WithMessage("Invalid tenant status.");
        }
    }
}
