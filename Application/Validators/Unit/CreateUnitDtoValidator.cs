using Application.Dto.Unit;
using Domain.Enums;
using FluentValidation;

namespace Application.Validators.Unit
{
    public class CreateUnitDtoValidator: AbstractValidator<CreateUnitDto>
    {
        public CreateUnitDtoValidator()
        {
            RuleFor(x => x.UnitNo)
                .NotEmpty().WithMessage("Unit number is required.")
                .MaximumLength(10).WithMessage("Unit number cannot exceed 10 characters.");

            RuleFor(x => x.Floor)
               .NotEmpty().WithMessage("Floor is required.")
               .Must(BeAValidInteger).WithMessage("Floor must be a valid number.") // The method below fixes the error
               .DependentRules(() => {
                   RuleFor(x => x.Floor)
                       .Must(floor => int.Parse(floor) >= -2 && int.Parse(floor) <= 100)
                       .WithMessage("Floor must be between -2 and 100.");
               });

            RuleFor(x => x.RentPrice)
                .GreaterThan(0).WithMessage("Rent price must be a positive number.");

            RuleFor(x => x.PropertyId)
                .NotEmpty().WithMessage("A unit must be linked to a Property.");

            RuleFor(x => (int)x.UnitStatus) // Cast to int for broader compatibility
    .Must(val => Enum.IsDefined(typeof(UnitStatus), (byte)val))
    .WithMessage("Invalid unit status.");
        }
        private bool BeAValidInteger(string floor)
        {
            return int.TryParse(floor, out _);
        }
    }

}
