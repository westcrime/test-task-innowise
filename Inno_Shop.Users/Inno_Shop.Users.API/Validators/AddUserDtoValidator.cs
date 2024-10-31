using FluentValidation;
using Inno_Shop.Users.API.DTOs;
using Inno_Shop.Users.Domain.Entities;

namespace Inno_Shop.Users.API.Validators
{
    public class AddUserDtoValidator: AbstractValidator<AddUserDto>
    {
        public AddUserDtoValidator() 
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(40);

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email format.")
                .MaximumLength(40);

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required.")
                .MinimumLength(8).WithMessage("Password must be at least 8 characters long.")
                .Matches(@"[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
                .Matches(@"[a-z]").WithMessage("Password must contain at least one lowercase letter.")
                .Matches(@"[0-9]").WithMessage("Password must contain at least one number.")
                .Matches(@"[\!\?\*\.]").WithMessage("Password must contain at least one special character (!? * .).")
                .MaximumLength(100);

            RuleFor(x => x.Role)
                .Must(role => Enum.IsDefined(typeof(Roles), role)).WithMessage("Invalid role.")
                .MaximumLength(40);
        }
    }
}
