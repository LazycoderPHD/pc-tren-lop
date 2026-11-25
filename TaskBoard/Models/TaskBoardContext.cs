using System.Data.Entity;

namespace TaskBoard.Models
{
    // DbContext ánh xạ tới cơ sở dữ liệu TaskBoardDb đã có sẵn (tạo bằng Database/TaskBoardDb.sql).
    public class TaskBoardContext : DbContext
    {
        public TaskBoardContext() : base("name=TaskBoardContext")
        {
            // Initializer = null: Entity Framework không tự tạo hay sửa cơ sở dữ liệu,
            // vì cấu trúc bảng đã được tạo sẵn bằng script SQL.
            Database.SetInitializer<TaskBoardContext>(null);

            // Tắt lazy loading để mọi truy vấn đều rõ ràng: dữ liệu liên quan chỉ được nạp khi gọi Include.
            this.Configuration.LazyLoadingEnabled = false;
            this.Configuration.ProxyCreationEnabled = false;
        }

        public DbSet<AppUser> Users { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<ProjectMember> ProjectMembers { get; set; }
        public DbSet<BoardColumn> BoardColumns { get; set; }
        public DbSet<Priority> Priorities { get; set; }
        public DbSet<TaskItem> Tasks { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<ActivityLog> ActivityLogs { get; set; }
    }
}
