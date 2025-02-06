namespace ToDo_Backend_CA_IntefaceAdapters_Presenters.Views
{
    public class TaskViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public string CreateDate { get; set; }
        public string UpdateDate { get; set; }
        public string DueDate { get; set; }
        public string IsPublic { get; set; }
    }
}
