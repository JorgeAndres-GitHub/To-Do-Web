using FluentValidation;
using ToDo_Backend_InterfaceAdapters_Mappers.DTOs.Requests.Task;

namespace ToDo_Backend_FrameworksDrivers_API.Validators.Task
{
    public class AddTaskValidator : AbstractValidator<TaskRequestDTO>
    {
        public AddTaskValidator()
        {
            RuleFor(dto => dto.Title).NotEmpty().WithMessage("The title is required and cannot be empty.")
                .MaximumLength(100).WithMessage("The title cannot exceed 100 characters.");

            RuleFor(dto => dto.Description).MaximumLength(500).WithMessage("The description cannot exceed 500 characters.");

            RuleFor(dto => dto.DueDate).GreaterThanOrEqualTo(DateOnly.FromDateTime(DateTime.Today))
                .When(x => x.DueDate.HasValue).WithMessage("The due date must be greater than or equal to the current date.");
        }
    }
}
