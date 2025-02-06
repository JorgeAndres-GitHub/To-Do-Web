using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToDo_Backend_CA_AplicationLayer.Interfaces;
using ToDo_Backend_CA_AplicationLayer.Interfaces.TaskAplicationInterfaces;
using ToDo_Backend_CA_EnterpriseLayer;
using ToDo_Backend_CA_IntefaceAdapters_Presenters.Views;

namespace ToDo_Backend_CA_IntefaceAdapters_Presenters.Presenters
{
    public class TaskPresenter : IPresenter<TaskItem, TaskViewModel>
    {
        public IEnumerable<TaskViewModel> Present(IEnumerable<TaskItem> tasks)
            => tasks.Select(t => new TaskViewModel
            {
                Id = t.Id,
                Title = t.Title,
                Description = t.Description,
                Status = t.IsCompleted ? "Completed" : "Pending",
                CreateDate = t.CreatedAt.ToString("MMMM dd, yyyy"),
                UpdateDate = t.IsNullUpdatedAt(),
                DueDate = t.DueDate.HasValue ? t.DueDate.Value.ToString("MMMM dd, yyyy") : "No Due Date",
                IsPublic = t.IsPublic ? "Public" : "Private"
            });
    }
}
