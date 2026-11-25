using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskBoard.Models
{
    // Mỗi lớp trong file này ánh xạ một bảng của cơ sở dữ liệu TaskBoardDb (xem Database/TaskBoardDb.sql).
    // Đặt tên AppUser (không phải User) để không trùng với thuộc tính Controller.User.

    [Table("Users")]
    public class AppUser
    {
        [Key]
        public int UserId { get; set; }

        [Required]
        [StringLength(50)]
        public string Username { get; set; }

        [Required]
        [StringLength(100)]
        public string PasswordHash { get; set; }

        [Required]
        [StringLength(50)]
        public string PasswordSalt { get; set; }

        [Required]
        [StringLength(100)]
        public string FullName { get; set; }

        [Required]
        [StringLength(100)]
        public string Email { get; set; }

        [Required]
        [StringLength(20)]
        public string Role { get; set; }

        public bool IsActive { get; set; }

        public DateTime CreatedAt { get; set; }
    }

    [Table("Projects")]
    public class Project
    {
        public Project()
        {
            // Khởi tạo sẵn danh sách để có thể gọi Add() mà không bị lỗi null
            this.Columns = new List<BoardColumn>();
            this.Members = new List<ProjectMember>();
        }

        [Key]
        public int ProjectId { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        public int OwnerId { get; set; }

        [ForeignKey("OwnerId")]
        public virtual AppUser Owner { get; set; }

        public DateTime CreatedAt { get; set; }

        public virtual ICollection<BoardColumn> Columns { get; set; }

        public virtual ICollection<ProjectMember> Members { get; set; }
    }

    // Bảng trung gian của quan hệ nhiều-nhiều giữa Projects và Users, khóa chính gồm hai cột
    [Table("ProjectMembers")]
    public class ProjectMember
    {
        [Key, Column(Order = 0)]
        public int ProjectId { get; set; }

        [Key, Column(Order = 1)]
        public int UserId { get; set; }

        public DateTime JoinedAt { get; set; }

        [ForeignKey("ProjectId")]
        public virtual Project Project { get; set; }

        [ForeignKey("UserId")]
        public virtual AppUser User { get; set; }
    }

    [Table("BoardColumns")]
    public class BoardColumn
    {
        public BoardColumn()
        {
            this.Tasks = new List<TaskItem>();
        }

        [Key]
        public int ColumnId { get; set; }

        public int ProjectId { get; set; }

        [ForeignKey("ProjectId")]
        public virtual Project Project { get; set; }

        [Required]
        [StringLength(50)]
        public string Title { get; set; }

        public int SortOrder { get; set; }

        public virtual ICollection<TaskItem> Tasks { get; set; }
    }

    [Table("Priorities")]
    public class Priority
    {
        [Key]
        public int PriorityId { get; set; }

        [Required]
        [StringLength(30)]
        public string Name { get; set; }

        public int PriorityLevel { get; set; }

        [Required]
        [StringLength(7)]
        public string ColorCode { get; set; }
    }

    // Đặt tên TaskItem để không trùng với System.Threading.Tasks.Task
    [Table("Tasks")]
    public class TaskItem
    {
        public TaskItem()
        {
            this.Comments = new List<Comment>();
        }

        [Key]
        public int TaskId { get; set; }

        public int ColumnId { get; set; }

        [ForeignKey("ColumnId")]
        public virtual BoardColumn BoardColumn { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; }

        [StringLength(2000)]
        public string Description { get; set; }

        public int PriorityId { get; set; }

        [ForeignKey("PriorityId")]
        public virtual Priority Priority { get; set; }

        // int? vì cột AssigneeId cho phép NULL (thẻ chưa giao cho ai)
        public int? AssigneeId { get; set; }

        [ForeignKey("AssigneeId")]
        public virtual AppUser Assignee { get; set; }

        public int CreatedById { get; set; }

        [ForeignKey("CreatedById")]
        public virtual AppUser CreatedBy { get; set; }

        public DateTime? DueDate { get; set; }

        public int SortOrder { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? CompletedAt { get; set; }

        public virtual ICollection<Comment> Comments { get; set; }
    }

    [Table("Comments")]
    public class Comment
    {
        [Key]
        public int CommentId { get; set; }

        public int TaskId { get; set; }

        [ForeignKey("TaskId")]
        public virtual TaskItem Task { get; set; }

        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public virtual AppUser User { get; set; }

        [Required]
        [StringLength(1000)]
        public string Content { get; set; }

        public DateTime CreatedAt { get; set; }
    }

    [Table("ActivityLogs")]
    public class ActivityLog
    {
        [Key]
        public int LogId { get; set; }

        public int ProjectId { get; set; }

        [ForeignKey("ProjectId")]
        public virtual Project Project { get; set; }

        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public virtual AppUser User { get; set; }

        [Required]
        [StringLength(500)]
        public string Message { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
