using FluentValidation;
using ToDo_Backend_InterfaceAdapters_Mappers.DTOs.Requests.UserRequests;

namespace ToDo_Backend_FrameworksDrivers_API.Validators.UserValidators
{
    public class UpdateUserValidator :AbstractValidator<UpdateUserRequestDto>
    {
        public UpdateUserValidator()
        {
            RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First name is required juas juas.")
            .MaximumLength(50).WithMessage("First name cannot exceed 50 characters.").When(x => x.FirstName != null);

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Last name is required.")
                .MaximumLength(50).WithMessage("Last name cannot exceed 50 characters.");

            RuleFor(x => x.IdentificationNumber)
                .NotEmpty().WithMessage("Identification number is required.")
                .Matches(@"^\d+$").WithMessage("Identification number must contain only digits.")
                .MaximumLength(20).WithMessage("Identification number cannot exceed 20 characters.");

            RuleFor(x => x.Country)
                .NotEmpty().WithMessage("Country is required.")
                .MaximumLength(50).WithMessage("Country cannot exceed 50 characters.");

            RuleFor(x => x.City)
                .NotEmpty().WithMessage("City is required.")
                .MaximumLength(50).WithMessage("City cannot exceed 50 characters.");

            RuleFor(x => x.Phone)
                .NotEmpty().WithMessage("Phone number is required.")
                .Matches(@"^\+?\d{7,15}$").WithMessage("Phone number must contain between 7 and 15 digits and may include a '+' prefix.");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Must be a valid email address.");
        }
    }
}
