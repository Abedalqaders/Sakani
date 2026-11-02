using Application.Dto.Expense;
using Domain.Enums;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Validators.Expense
{
    public class UpdateExpenseDtoValidator:AbstractValidator<UpdateExpenseDto>
    {
        public UpdateExpenseDtoValidator()
        {
            RuleFor(x => x.Amount)
                .GreaterThan(0)
                .When(x => x.Amount.HasValue);

            RuleFor(x => x.Description)
                .MaximumLength(500)
                .When(x => !string.IsNullOrEmpty(x.Description));

            RuleFor(x => x.ExpenseType)
                .IsInEnum()
                .WithMessage("Invalid Expense Type.")
                .When(x => x.ExpenseType.HasValue);
        }
    }
}
