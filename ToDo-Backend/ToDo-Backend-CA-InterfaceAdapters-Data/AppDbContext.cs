using Microsoft.EntityFrameworkCore;
using ToDo_Backend_InterfaceAdapters_Models;

namespace ToDo_Backend_CA_InterfaceAdapters_Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<TaskModel> Tasks { get; set; }
        public DbSet<UserModel> Users { get; set; }
        public DbSet<UserTaskModel> UserTaskModels { get; set; }
        public DbSet<RoleModel> Roles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TaskModel>().ToTable("Task");
            modelBuilder.Entity<UserModel>().ToTable("Account");
            modelBuilder.Entity<RoleModel>().ToTable("Role");  
            modelBuilder.Entity<UserTaskModel>().ToTable("UserTaskModel");

            modelBuilder.Entity<UserModel>()
                .HasKey(u => u.Id);

            modelBuilder.Entity<UserModel>()
                .HasOne(u => u.Role)
                .WithMany(r => r.Users)
                .HasForeignKey(u => u.IdRol);

            modelBuilder.Entity<RoleModel>()
                .HasKey(r => r.Id);

            modelBuilder.Entity<TaskModel>()
                .HasKey(t => t.Id);

            modelBuilder.Entity<UserTaskModel>()
                .HasKey(ut => ut.Id);

            modelBuilder.Entity<UserTaskModel>()
                .HasOne(ut => ut.User)
                .WithMany(u => u.UserTasks)
                .HasForeignKey(ut => ut.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserTaskModel>()
                .HasOne(ut => ut.Task)
                .WithMany(t => t.UserTasks)
                .HasForeignKey(ut => ut.TaskId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<TaskModel>()
                .HasMany(t => t.UserTasks)
                .WithOne(ut => ut.Task)
                .HasForeignKey(ut => ut.TaskId).
                OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserModel>()
                .HasMany(u => u.UserTasks)
                .WithOne(ut => ut.User)
                .HasForeignKey(ut => ut.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }

    }
}
