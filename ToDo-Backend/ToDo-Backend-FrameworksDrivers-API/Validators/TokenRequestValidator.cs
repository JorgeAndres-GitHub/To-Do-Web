using FluentValidation;
using ToDo_Backend_InterfaceAdapters_Mappers.DTOs;

namespace ToDo_Backend_FrameworksDrivers_API.Validators
{
    public class TokenRequestValidator : AbstractValidator<TokenRequestDto>
    {
        public TokenRequestValidator() 
        {
            RuleFor(x => x.Token)
                .NotEmpty().WithMessage("Token is required")
                .NotNull().WithMessage("Token is required");
            RuleFor(x => x.RefreshToken)
                .NotEmpty().WithMessage("Refresh Token is required")
                .NotNull().WithMessage("Refresh Token is required");
        }
    }
}
