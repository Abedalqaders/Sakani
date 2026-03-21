
using FluentValidation;
using Application.Dto.Property;
namespace Application.Validators.Property
{
    public class CreatePropertyDtoValidator: AbstractValidator<CreatePropertyDto>
    {
        public CreatePropertyDtoValidator()
        {
            RuleFor(p => p.Name)
            .NotEmpty().WithMessage("Property name is required.")
            .MaximumLength(100).WithMessage("Name cannot exceed 100 characters.");

            RuleFor(p => p.City)
                .NotEmpty().WithMessage("City is required.");

            RuleFor(p => p.BuildingNo)
                .NotEmpty().WithMessage("Building number is required.")
                .MaximumLength(20).WithMessage("Building number is too long.");

            RuleFor(p => p.PropertyType)
                .IsInEnum().WithMessage("Invalid Property Type.");
        }
    }
}
