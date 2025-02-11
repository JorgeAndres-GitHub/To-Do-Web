using FluentValidation;
using ToDo_Backend_InterfaceAdapters_Mappers.DTOs.Requests.User;

namespace ToDo_Backend_FrameworksDrivers_API.Validators.User
{
    public class RegisterValidator : AbstractValidator<UserRegistrationRequestDto>
    {
        public RegisterValidator()
        {
            RuleFor(x => x.FirstName).NotEmpty().WithMessage("First Name is required").MaximumLength(50).WithMessage("First Name can't be longer than 50 characters");
            RuleFor(x => x.LastName).NotEmpty().WithMessage("Last Name is required").MaximumLength(50).WithMessage("Last Name can't be longer than 50 characters");
            RuleFor(x => x.IdentificationNumber).NotEmpty().WithMessage("Identification Number is required").Length(6, 20).WithMessage("Identification number must be between 6 and 20 characters.");
            RuleFor(x => x.Country).NotEmpty().WithMessage("Country is required").MaximumLength(50).WithMessage("Country can't be longer than 50 characters");
            RuleFor(x => x.City).NotEmpty().WithMessage("City is required").MaximumLength(50).WithMessage("City can't be longer than 50 characters");
            RuleFor(x => x.Phone).NotEmpty().WithMessage("Phone is required").Matches(@"^\+?[0-9]{10,15}$").WithMessage("Phone number must be valid and contain between 10 and 15 digits.");
            RuleFor(x => x.Email).NotEmpty().WithMessage("Email is required").EmailAddress().WithMessage("Email must be valid");
            RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(8).WithMessage("Password must be at least 8 characters long.")
            .Matches(@"[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
            .Matches(@"[a-z]").WithMessage("Password must contain at least one lowercase letter.")
            .Matches(@"[0-9]").WithMessage("Password must contain at least one digit.")
            .Matches(@"[\@\#\$\%\^\&\*\(\)\-\=\+\!\.\~]").WithMessage("Password must contain at least one special character.");
        }
    }
}
