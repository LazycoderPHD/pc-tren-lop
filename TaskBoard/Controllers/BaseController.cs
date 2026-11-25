using System;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using TaskBoard.Models;

namespace TaskBoard.Controllers
{
    // Lớp cha của mọi Controller: giữ DbContext dùng chung và các hàm kiểm tra quyền truy cập.
    public class BaseController : Controller
    {
        protected TaskBoardContext db = new TaskBoardContext();

        private AppUser currentUser;

        // Tài khoản đang đăng nhập, đọc từ cơ sở dữ liệu (chỉ truy vấn một lần cho mỗi request)
        protected AppUser CurrentUser
        {
            get
            {
                if (this.currentUser == null && this.User.Identity.IsAuthenticated)
                {
                    string username = this.User.Identity.Name;
                    this.currentUser = this.db.Users.FirstOrDefault(u => u.Username == username);
                }
                return this.currentUser;
            }
        }

        // Chạy trước mọi action. Cookie đăng nhập chỉ nhớ tên đăng nhập và vai trò, nên ở đây đối chiếu lại
        // với cơ sở dữ liệu: nếu tài khoản bị khóa, bị xóa hoặc bị đổi vai trò thì buộc đăng nhập lại.
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (this.User.Identity.IsAuthenticated)
            {
                AppUser account = this.CurrentUser;
                bool valid = account != null && account.IsActive && this.User.IsInRole(account.Role);
                if (!valid)
                {
                    FormsAuthentication.SignOut();
                    filterContext.Result = this.RedirectToAction("Login", "Account");
                    return;
                }
            }
            base.OnActionExecuting(filterContext);
        }

        protected bool IsAdmin()
        {
            return this.User.IsInRole("Admin");
        }

        // Admin xem được mọi dự án. Member chỉ xem dự án mình sở hữu hoặc là thành viên.
        protected bool CanAccessProject(int projectId)
        {
            if (this.IsAdmin())
            {
                return true;
            }
            int userId = this.CurrentUser.UserId;
            return this.db.Projects.Any(p => p.ProjectId == projectId
                && (p.OwnerId == userId || p.Members.Any(m => m.UserId == userId)));
        }

        // Chỉ chủ dự án hoặc Admin được sửa, xóa dự án và quản lý thành viên
        protected bool CanManageProject(int projectId)
        {
            if (this.IsAdmin())
            {
                return true;
            }
            int userId = this.CurrentUser.UserId;
            return this.db.Projects.Any(p => p.ProjectId == projectId && p.OwnerId == userId);
        }

        // Cột có SortOrder lớn nhất của dự án được coi là cột "Hoàn thành"
        protected bool IsDoneColumn(BoardColumn column)
        {
            int maxOrder = this.db.BoardColumns.Where(c => c.ProjectId == column.ProjectId).Max(c => c.SortOrder);
            return column.SortOrder == maxOrder;
        }

        // Ghi một dòng nhật ký hoạt động. Hàm gọi phải gọi db.SaveChanges() sau đó.
        protected void AddLog(int projectId, string message)
        {
            ActivityLog log = new ActivityLog();
            log.ProjectId = projectId;
            log.UserId = this.CurrentUser.UserId;
            log.Message = message;
            log.CreatedAt = DateTime.Now;
            this.db.ActivityLogs.Add(log);
        }

        protected ActionResult Forbidden()
        {
            this.Response.StatusCode = 403;
            this.Response.TrySkipIisCustomErrors = true;
            return this.View("Forbidden");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
