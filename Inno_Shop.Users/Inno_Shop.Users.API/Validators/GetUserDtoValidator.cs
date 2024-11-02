using FluentValidation;
using Inno_Shop.Users.API.DTOs;

namespace Inno_Shop.Users.API.Validators
{
    public class GetUserDtoValidator : AbstractValidator<GetUserDto>
    {
        public GetUserDtoValidator() 
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email format.")
                .MaximumLength(40);
        }
    }
}
