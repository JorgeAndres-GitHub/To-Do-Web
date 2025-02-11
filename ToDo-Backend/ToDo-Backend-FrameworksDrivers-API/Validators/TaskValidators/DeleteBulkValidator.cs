using FluentValidation;
using ToDo_Backend_InterfaceAdapters_Mappers.DTOs.Requests.Task;

namespace ToDo_Backend_FrameworksDrivers_API.Validators.Task
{
    public class DeleteBulkValidator : AbstractValidator<BulkDeleteRequestDto>
    {
        public DeleteBulkValidator()
        {
            RuleFor(dto => dto.Ids).NotEmpty().WithMessage("The ids are required and cannot be empty.");
        }
    }
}
