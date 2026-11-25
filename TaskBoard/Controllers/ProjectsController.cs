using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using TaskBoard.Helpers;
using TaskBoard.Models;
using TaskBoard.Models.ViewModels;

namespace TaskBoard.Controllers
{
    // Chỉ cần đăng nhập. Quyền chi tiết trên từng dự án được kiểm tra trong từng action.
    [RoleAuthorize]
    public class ProjectsController : BaseController
    {
        // Danh sách dự án có tìm kiếm theo tên
        public ActionResult Index(string search)
        {
            IQueryable<Project> query = this.db.Projects;

            // Member chỉ thấy dự án mình sở hữu hoặc tham gia, Admin thấy tất cả
            if (!this.IsAdmin())
            {
                int userId = this.CurrentUser.UserId;
                query = query.Where(p => p.OwnerId == userId || p.Members.Any(m => m.UserId == userId));
            }

            if (!string.IsNullOrWhiteSpace(search))
            {
                string keyword = search.Trim();
                query = query.Where(p => p.Name.Contains(keyword));
            }

            List<ProjectListItem> items = query
                .OrderByDescending(p => p.CreatedAt)
                .Select(p => new ProjectListItem
                {
                    ProjectId = p.ProjectId,
                    Name = p.Name,
                    Description = p.Description,
                    OwnerName = p.Owner.FullName,
                    MemberCount = p.Members.Count(),
                    TaskCount = p.Columns.SelectMany(c => c.Tasks).Count()
                })
                .ToList();

            this.ViewBag.Search = search;
            return this.View(items);
        }

        // Trang bảng Kanban của một dự án
        public ActionResult Details(int id)
        {
            Project project = this.db.Projects.Include(p => p.Owner).FirstOrDefault(p => p.ProjectId == id);
            if (project == null)
            {
                return this.HttpNotFound();
            }
            if (!this.CanAccessProject(id))
            {
                return this.Forbidden();
            }

            BoardViewModel model = new BoardViewModel();
            model.Project = project;
            model.Columns = this.db.BoardColumns
                .Where(c => c.ProjectId == id)
                .OrderBy(c => c.SortOrder)
                .ToList();
            model.Tasks = this.db.Tasks
                .Include(t => t.Priority)
                .Include(t => t.Assignee)
                .Where(t => t.BoardColumn.ProjectId == id)
                .OrderBy(t => t.SortOrder)
                .ToList();
            model.Members = this.db.ProjectMembers
                .Include(m => m.User)
                .Where(m => m.ProjectId == id)
                .OrderBy(m => m.User.FullName)
                .ToList();
            model.Logs = this.db.ActivityLogs
                .Include(l => l.User)
                .Where(l => l.ProjectId == id)
                .OrderByDescending(l => l.CreatedAt)
                .Take(8)
                .ToList();
            model.CanManage = this.CanManageProject(id);
            return this.View(model);
        }

        [HttpGet]
        public ActionResult Create()
        {
            return this.View(new ProjectFormViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ProjectFormViewModel model)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(model);
            }

            int ownerId = this.CurrentUser.UserId;

            Project project = new Project();
            project.Name = model.Name.Trim();
            project.Description = model.Description;
            project.OwnerId = ownerId;
            project.CreatedAt = DateTime.Now;

            // Mỗi dự án mới có sẵn ba cột. Cột cuối cùng (SortOrder lớn nhất) là cột Hoàn thành.
            BoardColumn todo = new BoardColumn();
            todo.Title = "Cần làm";
            todo.SortOrder = 1;
            project.Columns.Add(todo);

            BoardColumn doing = new BoardColumn();
            doing.Title = "Đang làm";
            doing.SortOrder = 2;
            project.Columns.Add(doing);

            BoardColumn done = new BoardColumn();
            done.Title = "Hoàn thành";
            done.SortOrder = 3;
            project.Columns.Add(done);

            // Chủ dự án cũng là một thành viên của dự án
            ProjectMember ownerMember = new ProjectMember();
            ownerMember.UserId = ownerId;
            ownerMember.JoinedAt = DateTime.Now;
            project.Members.Add(ownerMember);

            this.db.Projects.Add(project);
            this.db.SaveChanges();

            this.AddLog(project.ProjectId, "đã tạo dự án");
            this.db.SaveChanges();

            this.TempData["Message"] = "Đã tạo dự án.";
            return this.RedirectToAction("Details", new { id = project.ProjectId });
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            Project project = this.db.Projects.Find(id);
            if (project == null)
            {
                return this.HttpNotFound();
            }
            if (!this.CanManageProject(id))
            {
                return this.Forbidden();
            }

            ProjectFormViewModel model = new ProjectFormViewModel();
            model.ProjectId = project.ProjectId;
            model.Name = project.Name;
            model.Description = project.Description;
            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ProjectFormViewModel model)
        {
            Project project = this.db.Projects.Find(model.ProjectId);
            if (project == null)
            {
                return this.HttpNotFound();
            }
            if (!this.CanManageProject(project.ProjectId))
            {
                return this.Forbidden();
            }
            if (!this.ModelState.IsValid)
            {
                return this.View(model);
            }

            project.Name = model.Name.Trim();
            project.Description = model.Description;
            this.AddLog(project.ProjectId, "đã cập nhật thông tin dự án");
            this.db.SaveChanges();

            this.TempData["Message"] = "Đã cập nhật dự án.";
            return this.RedirectToAction("Details", new { id = project.ProjectId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            Project project = this.db.Projects.Find(id);
            if (project == null)
            {
                return this.HttpNotFound();
            }
            if (!this.CanManageProject(id))
            {
                return this.Forbidden();
            }

            // Cơ sở dữ liệu có ON DELETE CASCADE nên cột, thẻ, bình luận, thành viên và nhật ký bị xóa theo
            this.db.Projects.Remove(project);
            this.db.SaveChanges();

            this.TempData["Message"] = "Đã xóa dự án.";
            return this.RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddMember(int id, string username)
        {
            if (!this.CanManageProject(id))
            {
                return this.Forbidden();
            }

            string name = string.Empty;
            if (username != null)
            {
                name = username.Trim();
            }

            AppUser account = this.db.Users.FirstOrDefault(u => u.Username == name && u.IsActive);
            if (account == null)
            {
                this.TempData["Error"] = "Không tìm thấy tài khoản đang hoạt động với tên đăng nhập này.";
                return this.RedirectToAction("Details", new { id = id });
            }

            int accountId = account.UserId;
            bool alreadyMember = this.db.ProjectMembers.Any(m => m.ProjectId == id && m.UserId == accountId);
            if (alreadyMember)
            {
                this.TempData["Error"] = "Người này đã là thành viên của dự án.";
                return this.RedirectToAction("Details", new { id = id });
            }

            ProjectMember member = new ProjectMember();
            member.ProjectId = id;
            member.UserId = accountId;
            member.JoinedAt = DateTime.Now;
            this.db.ProjectMembers.Add(member);

            this.AddLog(id, "đã thêm " + account.FullName + " vào dự án");
            this.db.SaveChanges();

            this.TempData["Message"] = "Đã thêm thành viên.";
            return this.RedirectToAction("Details", new { id = id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RemoveMember(int id, int userId)
        {
            if (!this.CanManageProject(id))
            {
                return this.Forbidden();
            }

            Project project = this.db.Projects.Find(id);
            if (project == null)
            {
                return this.HttpNotFound();
            }
            if (project.OwnerId == userId)
            {
                this.TempData["Error"] = "Không thể xóa chủ dự án khỏi danh sách thành viên.";
                return this.RedirectToAction("Details", new { id = id });
            }

            ProjectMember member = this.db.ProjectMembers.FirstOrDefault(m => m.ProjectId == id && m.UserId == userId);
            if (member == null)
            {
                return this.HttpNotFound();
            }

            // Các thẻ đang giao cho người này trở về trạng thái chưa giao
            List<TaskItem> assignedTasks = this.db.Tasks
                .Where(t => t.BoardColumn.ProjectId == id && t.AssigneeId == userId)
                .ToList();
            foreach (TaskItem task in assignedTasks)
            {
                task.AssigneeId = null;
            }

            AppUser removedUser = this.db.Users.Find(userId);
            this.db.ProjectMembers.Remove(member);
            this.AddLog(id, "đã xóa " + removedUser.FullName + " khỏi dự án");
            this.db.SaveChanges();

            this.TempData["Message"] = "Đã xóa thành viên.";
            return this.RedirectToAction("Details", new { id = id });
        }
    }
}
