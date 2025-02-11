using FluentValidation;
using ToDo_Backend_InterfaceAdapters_Mappers.DTOs.Requests.Task;

namespace ToDo_Backend_FrameworksDrivers_API.Validators.Task
{
    public class UpdateTaskValidator : AbstractValidator<UpdateTaskRequestDto>
    {
        public UpdateTaskValidator()
        {
            RuleFor(dto => dto.Id).NotEmpty().WithMessage("The id is required and cannot be empty.");
            RuleFor(dto => dto.Title).MaximumLength(100).WithMessage("The title cannot exceed 100 characters.");
            RuleFor(dto => dto.Description).MaximumLength(500).WithMessage("The description cannot exceed 500 characters.");
            RuleFor(dto => dto.DueDate).GreaterThanOrEqualTo(DateOnly.FromDateTime(DateTime.Today))
                .When(x => x.DueDate.HasValue).WithMessage("The due date must be greater than or equal to the current date.");
        }
    }
}
