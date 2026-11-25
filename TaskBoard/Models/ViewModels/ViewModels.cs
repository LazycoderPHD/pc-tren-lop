using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace TaskBoard.Models.ViewModels
{
    // Các lớp ViewModel dùng cho form và trang hiển thị. Data Annotation ở đây kiểm tra dữ liệu phía máy chủ
    // và đồng thời sinh ra kiểm tra phía trình duyệt (jquery.validate.unobtrusive).

    public class LoginViewModel
    {
        [Required(ErrorMessage = "Vui lòng nhập tên đăng nhập.")]
        [Display(Name = "Tên đăng nhập")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập mật khẩu.")]
        [DataType(DataType.Password)]
        [Display(Name = "Mật khẩu")]
        public string Password { get; set; }

        [Display(Name = "Ghi nhớ đăng nhập")]
        public bool RememberMe { get; set; }
    }

    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Vui lòng nhập tên đăng nhập.")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Tên đăng nhập dài từ 3 đến 50 ký tự.")]
        [RegularExpression("^[a-zA-Z0-9_]+$", ErrorMessage = "Tên đăng nhập chỉ gồm chữ cái không dấu, số và dấu gạch dưới.")]
        [Display(Name = "Tên đăng nhập")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập họ tên.")]
        [StringLength(100, ErrorMessage = "Họ tên tối đa 100 ký tự.")]
        [Display(Name = "Họ tên")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập email.")]
        [EmailAddress(ErrorMessage = "Email không đúng định dạng.")]
        [StringLength(100, ErrorMessage = "Email tối đa 100 ký tự.")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập mật khẩu.")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Mật khẩu dài từ 6 đến 100 ký tự.")]
        [DataType(DataType.Password)]
        [Display(Name = "Mật khẩu")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessage = "Mật khẩu nhập lại không khớp.")]
        [Display(Name = "Nhập lại mật khẩu")]
        public string ConfirmPassword { get; set; }
    }

    // Dùng cho trang quản lý người dùng của Admin (thêm và sửa)
    public class UserFormViewModel
    {
        public int UserId { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên đăng nhập.")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Tên đăng nhập dài từ 3 đến 50 ký tự.")]
        [RegularExpression("^[a-zA-Z0-9_]+$", ErrorMessage = "Tên đăng nhập chỉ gồm chữ cái không dấu, số và dấu gạch dưới.")]
        [Display(Name = "Tên đăng nhập")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập họ tên.")]
        [StringLength(100, ErrorMessage = "Họ tên tối đa 100 ký tự.")]
        [Display(Name = "Họ tên")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập email.")]
        [EmailAddress(ErrorMessage = "Email không đúng định dạng.")]
        [StringLength(100, ErrorMessage = "Email tối đa 100 ký tự.")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        // Không bắt buộc ở đây: khi tạo mới thì Controller kiểm tra, khi sửa thì để trống nếu không đổi mật khẩu
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Mật khẩu dài từ 6 đến 100 ký tự.")]
        [DataType(DataType.Password)]
        [Display(Name = "Mật khẩu")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn vai trò.")]
        [Display(Name = "Vai trò")]
        public string Role { get; set; }

        [Display(Name = "Tài khoản đang hoạt động")]
        public bool IsActive { get; set; }

        public List<SelectListItem> RoleList { get; set; }
    }

    public class ProjectFormViewModel
    {
        public int ProjectId { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên dự án.")]
        [StringLength(100, ErrorMessage = "Tên dự án tối đa 100 ký tự.")]
        [Display(Name = "Tên dự án")]
        public string Name { get; set; }

        [StringLength(500, ErrorMessage = "Mô tả tối đa 500 ký tự.")]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Mô tả")]
        public string Description { get; set; }
    }

    public class ProjectListItem
    {
        public int ProjectId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string OwnerName { get; set; }
        public int MemberCount { get; set; }
        public int TaskCount { get; set; }
    }

    public class TaskFormViewModel
    {
        public int TaskId { get; set; }

        public int ProjectId { get; set; }

        public string ProjectName { get; set; }

        [Display(Name = "Cột")]
        public int ColumnId { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tiêu đề thẻ.")]
        [StringLength(200, ErrorMessage = "Tiêu đề tối đa 200 ký tự.")]
        [Display(Name = "Tiêu đề")]
        public string Title { get; set; }

        [StringLength(2000, ErrorMessage = "Mô tả tối đa 2000 ký tự.")]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Mô tả")]
        public string Description { get; set; }

        [Display(Name = "Mức ưu tiên")]
        public int PriorityId { get; set; }

        [Display(Name = "Người thực hiện")]
        public int? AssigneeId { get; set; }

        [Display(Name = "Hạn chót")]
        [DataType(DataType.DateTime)]
        public DateTime? DueDate { get; set; }

        public bool CanDelete { get; set; }

        public List<SelectListItem> ColumnList { get; set; }
        public List<SelectListItem> PriorityList { get; set; }
        public List<SelectListItem> AssigneeList { get; set; }
    }

    public class BoardViewModel
    {
        public Project Project { get; set; }
        public List<BoardColumn> Columns { get; set; }
        public List<TaskItem> Tasks { get; set; }
        public List<ProjectMember> Members { get; set; }
        public List<ActivityLog> Logs { get; set; }
        public bool CanManage { get; set; }
    }

    public class TaskDetailsViewModel
    {
        public TaskItem Task { get; set; }
        public int ProjectId { get; set; }
        public string ProjectName { get; set; }
        public List<Comment> Comments { get; set; }
        public bool CanDelete { get; set; }
    }
}
