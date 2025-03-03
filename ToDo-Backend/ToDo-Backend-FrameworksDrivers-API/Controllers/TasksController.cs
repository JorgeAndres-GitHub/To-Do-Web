using Azure.Core;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ToDo_Backend_CA_AplicationLayer.Exceptions;
using ToDo_Backend_CA_AplicationLayer.UseCases.TaskUseCases;
using ToDo_Backend_CA_EnterpriseLayer;
using ToDo_Backend_CA_IntefaceAdapters_Presenters.Views;
using ToDo_Backend_FrameworksDrivers_API.Middlewares;
using ToDo_Backend_FrameworksDrivers_API.Services;
using ToDo_Backend_InterfaceAdapters_Mappers.DTOs.Requests.Task;

namespace ToDo_Backend_FrameworksDrivers_API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class TasksController : ControllerBase
    {
        private readonly AddTaksUseCase<TaskRequestDto> _addTaskUseCase;
        private readonly GetTaskUseCase<TaskItem> _getTaskUseCase;
        private readonly GetAllTasksUseCase<TaskItem, TaskViewModel> _getAllTasksUseCase;
        private readonly DeleteTaskUseCase _deleteTaskUseCase;
        private readonly DeleteMultipleTasksUseCase _deleteMultipleTasksUseCase;
        private readonly UpdateTaskUseCase<UpdateTaskRequestDto> _updateTaskUseCase;
        private readonly MarkAsCompletedUseCase _markAsCompletedUseCase;
        private readonly GetAllUserTasksUseCase<TaskItem, TaskViewModel> _getAllUserTasksUseCase;
        private readonly PostTaskUseCase _postTaskUseCase;
        private readonly ILogger<TasksController> _logger;

        public TasksController(AddTaksUseCase<TaskRequestDto> addTaksUseCase, 
            GetTaskUseCase<TaskItem> getTaskUseCase, GetAllTasksUseCase<TaskItem, 
                TaskViewModel> getAllTasksUseCase, DeleteTaskUseCase deleteTaskUseCase,
            DeleteMultipleTasksUseCase deleteMultipleTasksUseCase, UpdateTaskUseCase<UpdateTaskRequestDto> updateTaskUseCase,
            MarkAsCompletedUseCase markAsCompletedUseCase, GetAllUserTasksUseCase<TaskItem, TaskViewModel> getAllUserTasksUseCase, PostTaskUseCase postTaskUseCase, 
            ILogger<TasksController> logger
            )
        {
            _addTaskUseCase = addTaksUseCase;
            _getTaskUseCase = getTaskUseCase;
            _getAllTasksUseCase = getAllTasksUseCase;   
            _deleteTaskUseCase = deleteTaskUseCase;
            _deleteMultipleTasksUseCase = deleteMultipleTasksUseCase;
            _updateTaskUseCase = updateTaskUseCase;
            _markAsCompletedUseCase = markAsCompletedUseCase;
            _getAllUserTasksUseCase = getAllUserTasksUseCase;
            _postTaskUseCase = postTaskUseCase;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] TaskRequestDto task)
        {          
            var userId = GetUserIdService.GetUserId(User);

            _logger.LogInformation($@"Create task request received for:
                                     - User id: {userId}.");

            var (taskId, shouldRefreshToken) = await _addTaskUseCase.ExecuteAsync(task, userId);
            return CreatedAtAction(nameof(GetById), new { id = taskId }, new {taskId, shouldRefreshToken});
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {           

            var userId = GetUserIdService.GetUserId(User);

            _logger.LogInformation($@"Looking for task with
                                - User id: {userId}.
                                - Task id: {id}");

            var task = await _getTaskUseCase.ExecuteAsync(id, userId);
            return Ok(task);            
        }

        [AllowAnonymous]
        [HttpGet("getAllTasks")]
        public async Task<IActionResult> GetAllTasks()
        {
            _logger.LogInformation("Getting all tasks.");

            var tasks = await _getAllTasksUseCase.ExecuteAsync();
            return Ok(tasks);
        }

        [HttpGet("getAllUserTasks")]
        public async Task<IActionResult> GetAllUserTasks()
        {           

            var userId = GetUserIdService.GetUserId(User);

            _logger.LogInformation($"Getting all user tasks for user id: {userId}.");

            var tasks = await _getAllUserTasksUseCase.ExecuteAsync(userId);
            return Ok(tasks); 
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTask(int id)
        {
            var userId = GetUserIdService.GetUserId(User);

            _logger.LogInformation($@"Delete user task with 
                                  - User id: {userId}.
                                  - Task id: {id}");

            await _deleteTaskUseCase.ExecuteAsync(id, userId);
            return NoContent();
        }

        [HttpDelete("bulk")]
        public async Task<IActionResult> DeleteMultipleTasks([FromBody] BulkDeleteRequestDto request)
        {
            var userId = GetUserIdService.GetUserId(User);

            var taskIds = string.Join(", ", request.Ids);
            _logger.LogInformation("Bulk delete user task with: User id: {UserId}, Tasks ids: {TaskIds}", userId, taskIds);


            await _deleteMultipleTasksUseCase.ExecuteAsync(request.Ids, userId);
            return NoContent();
        }

        [HttpPut]
        public async Task<IActionResult> UpdateTask([FromBody] UpdateTaskRequestDto request)
        {
            var userId = GetUserIdService.GetUserId(User);

            _logger.LogInformation($"Update user task with: User id: {userId}. Task Id: {request.Id.ToString().Replace('\n', '_').Replace('\r', '_')}");

            await _updateTaskUseCase.ExecuteAsync(request, userId);
            return NoContent();            
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> MarkAsCompletedTask(int id)
        {
            var userId = GetUserIdService.GetUserId(User);

            _logger.LogInformation($@"Mark as completed user task with:
                                  - User id: {userId}.
                                  - Task Id: {id}");

            var shouldRefreshToken = await _markAsCompletedUseCase.ExecuteAsync(id, userId);
            return Ok(new { shouldRefreshToken });            
        }

        [Authorize(Policy = "TaskPublisher")]
        [HttpPut("{taskId}")]
        public async Task<IActionResult> PostTask(int taskId)
        {
            await _postTaskUseCase.ExecuteAsync(taskId);
            return NoContent();
        }
    }
}
