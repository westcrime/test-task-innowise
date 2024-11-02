using FluentValidation;
using Inno_Shop.Users.API.DTOs;

namespace Inno_Shop.Users.API.Validators
{
    public class UpdateUserDtoValidator : AbstractValidator<UpdateUserDto>
    {

        public UpdateUserDtoValidator() 
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(40)
                .When(x => x.Name != null);

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required.")
                .MinimumLength(8).WithMessage("Password must be at least 8 characters long.")
                .Matches(@"[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
                .Matches(@"[a-z]").WithMessage("Password must contain at least one lowercase letter.")
                .Matches(@"[0-9]").WithMessage("Password must contain at least one number.")
                .Matches(@"[\!\?\*\.]").WithMessage("Password must contain at least one special character (!? * .).")
                .MaximumLength(100)
                .When(x => x.Password != null);
        }
    }
}
