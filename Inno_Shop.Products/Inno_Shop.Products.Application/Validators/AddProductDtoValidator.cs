using FluentValidation;
using Inno_Shop.Products.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inno_Shop.Products.Application.Validators
{
    public class AddProductDtoValidator : AbstractValidator<AddProductDto>
    {
        public AddProductDtoValidator() 
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(50);

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Description is required.")
                .MaximumLength(500);

            RuleFor(x => x.Cost)
                .NotEmpty().WithMessage("Cost is required.")
                .Must(cost => cost > 0);

            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("User id is required.");
        }
    }
}
