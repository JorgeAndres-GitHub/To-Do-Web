namespace ToDo_Backend_CA_EnterpriseLayer
{
    public class TaskItem
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public bool IsCompleted { get; set; }
        public bool IsPublic { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DueDate { get; set; }
        public int UserId { get; set; }
        public UserEntity User { get; set; }

        public string IsNullUpdatedAt()
            => UpdatedAt.HasValue ? UpdatedAt.Value.ToString("MMMM dd, yyyy") : "No updates yet";

        public void MarkAsCompleted()
        {
            if (IsCompleted)
                throw new InvalidOperationException("The task is already completed");
            IsCompleted = true;
        }
    }
}
