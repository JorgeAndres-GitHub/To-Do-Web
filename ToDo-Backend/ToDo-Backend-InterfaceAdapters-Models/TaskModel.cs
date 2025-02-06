using System.ComponentModel.DataAnnotations.Schema;

namespace ToDo_Backend_InterfaceAdapters_Models
{
    public class TaskModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public bool IsCompleted { get; set; }
        public bool IsPublic { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; } = null;
        public DateTime? DueDate { get; set; }
        public virtual ICollection<UserTaskModel> UserTasks { get; set; }
    }
}
