using FluentValidation;
using Inno_Shop.Users.API.DTOs;

namespace Inno_Shop.Users.API.Validators
{
    public class VerifyUserDtoValidator : AbstractValidator<VerifyUserDto>
    {
        public VerifyUserDtoValidator() 
        {
            RuleFor(x => x.Code)
                .NotEmpty().WithMessage("Code is required.")
                .MaximumLength(40);
        }
    }
}