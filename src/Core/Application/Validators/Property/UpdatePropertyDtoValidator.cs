using Application.Dto.Property;
using FluentValidation;

namespace Application.Validators.Property;

public class UpdatePropertyDtoValidator : AbstractValidator<UpdatePropertyDto>
{
    public UpdatePropertyDtoValidator()
    {
        
        RuleFor(p => p.Id)
            .NotEmpty().WithMessage("Property ID is required.");

        RuleFor(p => p.Name)
            .NotEmpty().WithMessage("Property name is required.")
            .MaximumLength(100);

        RuleFor(p => p.City)
            .NotEmpty().WithMessage("City is required.");

        RuleFor(p => p.BuildingNo)
            .NotEmpty().WithMessage("Building number is required.");

        RuleFor(p => p.PropertyType)
            .IsInEnum().WithMessage("Invalid Property Type.");
    }
}