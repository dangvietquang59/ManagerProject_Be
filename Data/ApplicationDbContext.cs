using Microsoft.EntityFrameworkCore;

namespace ManagerProject.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<ProjectMember> ProjectMembers { get; set; }
        public DbSet<TaskItem> TaskItems { get; set; }  // 🛠️ Đổi tên để đồng nhất
        public DbSet<Status> Statuses { get; set; }
        public DbSet<Priority> Priorities { get; set; }
        public DbSet<TaskAssignment> TaskAssignments { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<FileItem> Files { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // 🛠️ Định nghĩa khóa chính cho TaskItem (nếu chưa có)
            modelBuilder.Entity<TaskItem>()
                .HasKey(t => t.TaskID);

            // 🛠️ Thiết lập khóa chính cho các bảng trung gian
            modelBuilder.Entity<RolePermission>()
                .HasKey(rp => new { rp.RoleID, rp.PermissionID });

            modelBuilder.Entity<ProjectMember>()
                .HasKey(pm => new { pm.ProjectID, pm.UserID });

            modelBuilder.Entity<TaskAssignment>()
                .HasKey(ta => new { ta.TaskID, ta.UserID });

            // 🛠️ Sửa quan hệ giữa User và Role
            modelBuilder.Entity<User>()
                .HasOne(u => u.Role)
                .WithMany(r => r.Users) // 🔥 Đã thêm danh sách Users
                .HasForeignKey(u => u.RoleID)
                .OnDelete(DeleteBehavior.Restrict);

            // 🛠️ Thiết lập quan hệ giữa TaskItem với Status và Priority
            modelBuilder.Entity<TaskItem>()
                .HasOne(t => t.Status)
                .WithMany()
                .HasForeignKey(t => t.StatusID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TaskItem>()
                .HasOne(t => t.Priority)
                .WithMany()
                .HasForeignKey(t => t.PriorityID)
                .OnDelete(DeleteBehavior.Restrict);

            // 🛠️ Thiết lập quan hệ giữa Project và User (CreatedBy)
            modelBuilder.Entity<Project>()
                .HasOne(p => p.Creator)
                .WithMany()
                .HasForeignKey(p => p.CreatedBy)
                .OnDelete(DeleteBehavior.Restrict);

            // 🛠️ Seed dữ liệu mặc định cho Roles
            modelBuilder.Entity<Role>().HasData(
                new Role { RoleID = 1, Name = "Admin" },
                new Role { RoleID = 2, Name = "Project Manager" },
                new Role { RoleID = 3, Name = "Developer" },
                new Role { RoleID = 4, Name = "Tester" }
            );

            // 🛠️ Seed dữ liệu mặc định cho Permissions
            modelBuilder.Entity<Permission>().HasData(
                new Permission { PermissionID = 1, Name = "Create Project" },
                new Permission { PermissionID = 2, Name = "Edit Project" },
                new Permission { PermissionID = 3, Name = "Delete Project" },
                new Permission { PermissionID = 4, Name = "Assign Task" }
            );

            // 🛠️ Seed dữ liệu mặc định cho Statuses
            modelBuilder.Entity<Status>().HasData(
                new Status { StatusID = 1, Name = "Pending" },
                new Status { StatusID = 2, Name = "In Progress" },
                new Status { StatusID = 3, Name = "Completed" },
                new Status { StatusID = 4, Name = "On Hold" }
            );

            // 🛠️ Seed dữ liệu mặc định cho Priorities
            modelBuilder.Entity<Priority>().HasData(
                new Priority { PriorityID = 1, Name = "Low" },
                new Priority { PriorityID = 2, Name = "Medium" },
                new Priority { PriorityID = 3, Name = "High" },
                new Priority { PriorityID = 4, Name = "Critical" }
            );
        }
    }
}
