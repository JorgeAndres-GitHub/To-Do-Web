using Azure.Core;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
        private readonly AddTaksUseCase<TaskRequestDTO> _addTaskUseCase;
        private readonly GetTaskUseCase<TaskItem> _getTaskUseCase;
        private readonly GetAllTasksUseCase<TaskItem, TaskViewModel> _getAllTasksUseCase;
        private readonly DeleteTaskUseCase _deleteTaskUseCase;
        private readonly DeleteMultipleTasksUseCase _deleteMultipleTasksUseCase;
        private readonly UpdateTaskUseCase<UpdateTaskRequestDTO> _updateTaskUseCase;
        private readonly MarkAsCompletedUseCase _markAsCompletedUseCase;
        private readonly GetAllUserTasksUseCase<TaskItem, TaskViewModel> _getAllUserTasksUseCase;

        public TasksController(AddTaksUseCase<TaskRequestDTO> addTaksUseCase, 
            GetTaskUseCase<TaskItem> getTaskUseCase, GetAllTasksUseCase<TaskItem, 
                TaskViewModel> getAllTasksUseCase, DeleteTaskUseCase deleteTaskUseCase,
            DeleteMultipleTasksUseCase deleteMultipleTasksUseCase, UpdateTaskUseCase<UpdateTaskRequestDTO> updateTaskUseCase,
            MarkAsCompletedUseCase markAsCompletedUseCase, GetAllUserTasksUseCase<TaskItem, TaskViewModel> getAllUserTasksUseCase
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
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] TaskRequestDTO task)
        {
            var userId = GetUserIdService.GetUserId(User);

            var taskId = await _addTaskUseCase.ExecuteAsync(task, userId);
            return CreatedAtAction(nameof(GetById), new { id = taskId }, null);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var userId = GetUserIdService.GetUserId(User);

            var task = await _getTaskUseCase.ExecuteAsync(id, userId);
            return Ok(task);            
        }

        [AllowAnonymous]
        [HttpGet("getAllTasks")]
        public async Task<IActionResult> GetAllTasks()
        {
            var tasks = await _getAllTasksUseCase.ExecuteAsync();
            return Ok(tasks);
        }

        [HttpGet("getAllUserTasks")]
        public async Task<IActionResult> GetAllUserTasks()
        {
            var userId = GetUserIdService.GetUserId(User);

            var tasks = await _getAllUserTasksUseCase.ExecuteAsync(userId);
            return Ok(tasks); 
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTask(int id)
        {
            var userId = GetUserIdService.GetUserId(User);

            await _deleteTaskUseCase.ExecuteAsync(id, userId);
            return NoContent();
        }

        [HttpDelete("bulk")]
        public async Task<IActionResult> DeleteMultipleTasks([FromBody] BulkDeleteRequestDTO request)
        {
            var userId = GetUserIdService.GetUserId(User);

            await _deleteMultipleTasksUseCase.ExecuteAsync(request.Ids, userId);
            return NoContent();            
        }

        [HttpPut]
        public async Task<IActionResult> UpdateTask([FromBody] UpdateTaskRequestDTO request)
        {
            var userId = GetUserIdService.GetUserId(User);

            await _updateTaskUseCase.ExecuteAsync(request, userId);
            return NoContent();            
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> MarkAsCompletedTask(int id)
        {
            var userId = GetUserIdService.GetUserId(User);

            await _markAsCompletedUseCase.ExecuteAsync(id, userId);
            return NoContent();            
        }
    }
}
