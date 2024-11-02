using FluentValidation;
using Inno_Shop.Users.API.DTOs;

namespace Inno_Shop.Users.API.Validators
{
    public class DeleteUserDtoValidator : AbstractValidator<DeleteUserDto>
    {
        public DeleteUserDtoValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email format.")
                .MaximumLength(40);
        }
    }
}
