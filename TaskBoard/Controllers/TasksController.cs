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
    [RoleAuthorize]
    public class TasksController : BaseController
    {
        // Nạp dữ liệu cho các ô chọn (cột, mức ưu tiên, người thực hiện) và tên dự án
        private void FillLists(TaskFormViewModel model)
        {
            model.ProjectName = this.db.Projects
                .Where(p => p.ProjectId == model.ProjectId)
                .Select(p => p.Name)
                .FirstOrDefault();

            List<BoardColumn> columns = this.db.BoardColumns
                .Where(c => c.ProjectId == model.ProjectId)
                .OrderBy(c => c.SortOrder)
                .ToList();
            model.ColumnList = new List<SelectListItem>();
            foreach (BoardColumn column in columns)
            {
                model.ColumnList.Add(new SelectListItem { Value = column.ColumnId.ToString(), Text = column.Title });
            }

            List<Priority> priorities = this.db.Priorities.OrderBy(p => p.PriorityLevel).ToList();
            model.PriorityList = new List<SelectListItem>();
            foreach (Priority priority in priorities)
            {
                model.PriorityList.Add(new SelectListItem { Value = priority.PriorityId.ToString(), Text = priority.Name });
            }

            List<ProjectMember> members = this.db.ProjectMembers
                .Include(m => m.User)
                .Where(m => m.ProjectId == model.ProjectId)
                .OrderBy(m => m.User.FullName)
                .ToList();
            model.AssigneeList = new List<SelectListItem>();
            model.AssigneeList.Add(new SelectListItem { Value = "", Text = "Chưa giao" });
            foreach (ProjectMember member in members)
            {
                model.AssigneeList.Add(new SelectListItem { Value = member.UserId.ToString(), Text = member.User.FullName });
            }
        }

        // Kiểm tra cột và người thực hiện có thuộc đúng dự án hay không (chống gửi dữ liệu giả mạo)
        private void ValidateProjectData(TaskFormViewModel model)
        {
            int projectId = model.ProjectId;
            int columnId = model.ColumnId;
            bool columnOk = this.db.BoardColumns.Any(c => c.ColumnId == columnId && c.ProjectId == projectId);
            if (!columnOk)
            {
                this.ModelState.AddModelError("ColumnId", "Cột không hợp lệ.");
            }

            bool priorityOk = this.db.Priorities.Any(p => p.PriorityId == model.PriorityId);
            if (!priorityOk)
            {
                this.ModelState.AddModelError("PriorityId", "Mức ưu tiên không hợp lệ.");
            }

            if (model.AssigneeId.HasValue)
            {
                int assigneeId = model.AssigneeId.Value;
                bool assigneeOk = this.db.ProjectMembers.Any(m => m.ProjectId == projectId && m.UserId == assigneeId);
                if (!assigneeOk)
                {
                    this.ModelState.AddModelError("AssigneeId", "Người thực hiện phải là thành viên của dự án.");
                }
            }
        }

        private int NextSortOrder(int columnId)
        {
            int? maxOrder = this.db.Tasks.Where(t => t.ColumnId == columnId).Max(t => (int?)t.SortOrder);
            if (maxOrder.HasValue)
            {
                return maxOrder.Value + 1;
            }
            return 1;
        }

        [HttpGet]
        public ActionResult Create(int projectId, int? columnId)
        {
            Project project = this.db.Projects.Find(projectId);
            if (project == null)
            {
                return this.HttpNotFound();
            }
            if (!this.CanAccessProject(projectId))
            {
                return this.Forbidden();
            }

            TaskFormViewModel model = new TaskFormViewModel();
            model.ProjectId = projectId;

            if (columnId.HasValue)
            {
                model.ColumnId = columnId.Value;
            }
            else
            {
                model.ColumnId = this.db.BoardColumns
                    .Where(c => c.ProjectId == projectId)
                    .OrderBy(c => c.SortOrder)
                    .Select(c => c.ColumnId)
                    .FirstOrDefault();
            }

            // Mặc định chọn mức ưu tiên Trung bình (PriorityLevel = 2)
            model.PriorityId = this.db.Priorities
                .Where(p => p.PriorityLevel == 2)
                .Select(p => p.PriorityId)
                .FirstOrDefault();

            this.FillLists(model);
            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(TaskFormViewModel model)
        {
            if (!this.CanAccessProject(model.ProjectId))
            {
                return this.Forbidden();
            }

            this.ValidateProjectData(model);
            if (model.DueDate.HasValue && model.DueDate.Value < DateTime.Now)
            {
                this.ModelState.AddModelError("DueDate", "Hạn chót phải nằm trong tương lai.");
            }
            if (!this.ModelState.IsValid)
            {
                this.FillLists(model);
                return this.View(model);
            }

            BoardColumn column = this.db.BoardColumns.Find(model.ColumnId);

            TaskItem task = new TaskItem();
            task.ColumnId = model.ColumnId;
            task.Title = model.Title.Trim();
            task.Description = model.Description;
            task.PriorityId = model.PriorityId;
            task.AssigneeId = model.AssigneeId;
            task.CreatedById = this.CurrentUser.UserId;
            task.DueDate = model.DueDate;
            task.SortOrder = this.NextSortOrder(model.ColumnId);
            task.CreatedAt = DateTime.Now;
            if (this.IsDoneColumn(column))
            {
                task.CompletedAt = DateTime.Now;
            }

            this.db.Tasks.Add(task);
            this.AddLog(model.ProjectId, "đã thêm thẻ \"" + task.Title + "\"");
            this.db.SaveChanges();

            this.TempData["Message"] = "Đã thêm thẻ.";
            return this.RedirectToAction("Details", "Projects", new { id = model.ProjectId });
        }

        public ActionResult Details(int id)
        {
            TaskItem task = this.db.Tasks
                .Include(t => t.Priority)
                .Include(t => t.Assignee)
                .Include(t => t.CreatedBy)
                .Include(t => t.BoardColumn)
                .FirstOrDefault(t => t.TaskId == id);
            if (task == null)
            {
                return this.HttpNotFound();
            }

            int projectId = task.BoardColumn.ProjectId;
            if (!this.CanAccessProject(projectId))
            {
                return this.Forbidden();
            }

            TaskDetailsViewModel model = new TaskDetailsViewModel();
            model.Task = task;
            model.ProjectId = projectId;
            model.ProjectName = this.db.Projects.Where(p => p.ProjectId == projectId).Select(p => p.Name).First();
            model.Comments = this.db.Comments
                .Include(c => c.User)
                .Where(c => c.TaskId == id)
                .OrderBy(c => c.CreatedAt)
                .ToList();
            model.CanDelete = this.CanManageProject(projectId) || task.CreatedById == this.CurrentUser.UserId;
            return this.View(model);
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            TaskItem task = this.db.Tasks.Find(id);
            if (task == null)
            {
                return this.HttpNotFound();
            }

            int projectId = this.db.BoardColumns
                .Where(c => c.ColumnId == task.ColumnId)
                .Select(c => c.ProjectId)
                .First();
            if (!this.CanAccessProject(projectId))
            {
                return this.Forbidden();
            }

            TaskFormViewModel model = new TaskFormViewModel();
            model.TaskId = task.TaskId;
            model.ProjectId = projectId;
            model.ColumnId = task.ColumnId;
            model.Title = task.Title;
            model.Description = task.Description;
            model.PriorityId = task.PriorityId;
            model.AssigneeId = task.AssigneeId;
            model.DueDate = task.DueDate;
            model.CanDelete = this.CanManageProject(projectId) || task.CreatedById == this.CurrentUser.UserId;
            this.FillLists(model);
            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(TaskFormViewModel model)
        {
            TaskItem task = this.db.Tasks.Find(model.TaskId);
            if (task == null)
            {
                return this.HttpNotFound();
            }

            // Mã dự án lấy từ cơ sở dữ liệu, không tin giá trị gửi lên từ form
            int taskColumnId = task.ColumnId;
            int projectId = this.db.BoardColumns
                .Where(c => c.ColumnId == taskColumnId)
                .Select(c => c.ProjectId)
                .First();
            if (!this.CanAccessProject(projectId))
            {
                return this.Forbidden();
            }
            model.ProjectId = projectId;

            this.ValidateProjectData(model);
            if (!this.ModelState.IsValid)
            {
                model.CanDelete = this.CanManageProject(projectId) || task.CreatedById == this.CurrentUser.UserId;
                this.FillLists(model);
                return this.View(model);
            }

            task.Title = model.Title.Trim();
            task.Description = model.Description;
            task.PriorityId = model.PriorityId;
            task.AssigneeId = model.AssigneeId;
            task.DueDate = model.DueDate;

            // Nếu đổi cột thì đưa thẻ xuống cuối cột mới và cập nhật trạng thái hoàn thành
            if (task.ColumnId != model.ColumnId)
            {
                BoardColumn newColumn = this.db.BoardColumns.Find(model.ColumnId);
                task.ColumnId = model.ColumnId;
                task.SortOrder = this.NextSortOrder(model.ColumnId);
                if (this.IsDoneColumn(newColumn))
                {
                    if (!task.CompletedAt.HasValue)
                    {
                        task.CompletedAt = DateTime.Now;
                    }
                }
                else
                {
                    task.CompletedAt = null;
                }
            }

            this.AddLog(projectId, "đã cập nhật thẻ \"" + task.Title + "\"");
            this.db.SaveChanges();

            this.TempData["Message"] = "Đã cập nhật thẻ.";
            return this.RedirectToAction("Details", "Projects", new { id = projectId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            TaskItem task = this.db.Tasks.Find(id);
            if (task == null)
            {
                return this.HttpNotFound();
            }

            int taskColumnId = task.ColumnId;
            int projectId = this.db.BoardColumns
                .Where(c => c.ColumnId == taskColumnId)
                .Select(c => c.ProjectId)
                .First();
            if (!this.CanAccessProject(projectId))
            {
                return this.Forbidden();
            }

            // Chủ dự án và Admin xóa được mọi thẻ, thành viên khác chỉ xóa được thẻ do chính mình tạo
            bool allowed = this.CanManageProject(projectId) || task.CreatedById == this.CurrentUser.UserId;
            if (!allowed)
            {
                return this.Forbidden();
            }

            string title = task.Title;
            this.db.Tasks.Remove(task);
            this.AddLog(projectId, "đã xóa thẻ \"" + title + "\"");
            this.db.SaveChanges();

            this.TempData["Message"] = "Đã xóa thẻ.";
            return this.RedirectToAction("Details", "Projects", new { id = projectId });
        }

        // Được gọi bằng AJAX khi kéo thả thẻ: lưu cột mới và vị trí mới của thẻ
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Move(int taskId, int columnId, int index)
        {
            TaskItem task = this.db.Tasks.Find(taskId);
            if (task == null)
            {
                return this.Json(new { success = false, message = "Không tìm thấy thẻ." });
            }

            BoardColumn oldColumn = this.db.BoardColumns.Find(task.ColumnId);
            BoardColumn newColumn = this.db.BoardColumns.Find(columnId);
            if (newColumn == null || newColumn.ProjectId != oldColumn.ProjectId)
            {
                return this.Json(new { success = false, message = "Cột đích không hợp lệ." });
            }
            if (!this.CanAccessProject(oldColumn.ProjectId))
            {
                return this.Json(new { success = false, message = "Bạn không có quyền thao tác trên dự án này." });
            }

            bool changedColumn = oldColumn.ColumnId != newColumn.ColumnId;

            // Lấy các thẻ đang có trong cột đích (không tính thẻ đang kéo), chèn thẻ vào đúng vị trí rồi đánh số lại
            List<TaskItem> siblings = this.db.Tasks
                .Where(t => t.ColumnId == columnId && t.TaskId != taskId)
                .OrderBy(t => t.SortOrder)
                .ToList();
            if (index < 0)
            {
                index = 0;
            }
            if (index > siblings.Count)
            {
                index = siblings.Count;
            }
            siblings.Insert(index, task);
            for (int i = 0; i < siblings.Count; i++)
            {
                siblings[i].SortOrder = i + 1;
            }

            task.ColumnId = columnId;
            bool isDone = this.IsDoneColumn(newColumn);
            if (isDone)
            {
                if (!task.CompletedAt.HasValue)
                {
                    task.CompletedAt = DateTime.Now;
                }
            }
            else
            {
                task.CompletedAt = null;
            }

            if (changedColumn)
            {
                this.AddLog(oldColumn.ProjectId, "đã chuyển thẻ \"" + task.Title + "\" từ \"" + oldColumn.Title + "\" sang \"" + newColumn.Title + "\"");
            }
            this.db.SaveChanges();

            return this.Json(new { success = true, isDone = isDone });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddComment(int taskId, string content)
        {
            TaskItem task = this.db.Tasks.Find(taskId);
            if (task == null)
            {
                return this.HttpNotFound();
            }

            int taskColumnId = task.ColumnId;
            int projectId = this.db.BoardColumns
                .Where(c => c.ColumnId == taskColumnId)
                .Select(c => c.ProjectId)
                .First();
            if (!this.CanAccessProject(projectId))
            {
                return this.Forbidden();
            }

            string text = string.Empty;
            if (content != null)
            {
                text = content.Trim();
            }
            if (text.Length == 0)
            {
                this.TempData["Error"] = "Bình luận không được để trống.";
                return this.RedirectToAction("Details", new { id = taskId });
            }
            if (text.Length > 1000)
            {
                this.TempData["Error"] = "Bình luận tối đa 1000 ký tự.";
                return this.RedirectToAction("Details", new { id = taskId });
            }

            Comment comment = new Comment();
            comment.TaskId = taskId;
            comment.UserId = this.CurrentUser.UserId;
            comment.Content = text;
            comment.CreatedAt = DateTime.Now;
            this.db.Comments.Add(comment);
            this.db.SaveChanges();

            return this.RedirectToAction("Details", new { id = taskId });
        }
    }
}
