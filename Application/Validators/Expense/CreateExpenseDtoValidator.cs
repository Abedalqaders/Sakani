using Application.Dto.Expense;
using Domain.Enums;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Validators.Expense
{
    public class CreateExpenseDtoValidator: AbstractValidator<CreateExpenseDto>
    {
        public CreateExpenseDtoValidator()
        {
            RuleFor(x => x.PropertyId).NotEmpty().WithMessage("PropertyId is required.");
            RuleFor(x => x.Amount).GreaterThan(0).WithMessage("Amount must be greater than zero.");
            RuleFor(x => x.Description).MaximumLength(500).WithMessage("Description cannot exceed 500 characters.");
            RuleFor(x => (int)x.ExpenseType).Must(val => Enum.IsDefined(typeof(ExpenseType), (byte)val))
                 .WithMessage("Invalid Expense Type.");
        }
    }
}
