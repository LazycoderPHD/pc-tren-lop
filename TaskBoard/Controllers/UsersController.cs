using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using TaskBoard.Helpers;
using TaskBoard.Models;
using TaskBoard.Models.ViewModels;

namespace TaskBoard.Controllers
{
    // Toàn bộ controller này chỉ dành cho Admin. Quyền được kiểm tra ở phía máy chủ,
    // nên Member gõ thẳng địa chỉ /Users cũng bị chặn (403).
    [RoleAuthorize(Roles = "Admin")]
    public class UsersController : BaseController
    {
        public ActionResult Index(string search, string role)
        {
            IQueryable<AppUser> query = this.db.Users;

            if (!string.IsNullOrWhiteSpace(search))
            {
                string keyword = search.Trim();
                query = query.Where(u => u.Username.Contains(keyword)
                    || u.FullName.Contains(keyword)
                    || u.Email.Contains(keyword));
            }
            if (!string.IsNullOrEmpty(role))
            {
                query = query.Where(u => u.Role == role);
            }

            List<AppUser> users = query.OrderBy(u => u.Role).ThenBy(u => u.FullName).ToList();
            this.ViewBag.Search = search;
            this.ViewBag.Role = role;
            return this.View(users);
        }

        [HttpGet]
        public ActionResult Create()
        {
            UserFormViewModel model = new UserFormViewModel();
            model.Role = "Member";
            model.IsActive = true;
            model.RoleList = this.BuildRoleList();
            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(UserFormViewModel model)
        {
            if (string.IsNullOrWhiteSpace(model.Password))
            {
                this.ModelState.AddModelError("Password", "Vui lòng nhập mật khẩu.");
            }
            if (model.Role != "Admin" && model.Role != "Member")
            {
                this.ModelState.AddModelError("Role", "Vai trò không hợp lệ.");
            }

            string username = string.Empty;
            if (model.Username != null)
            {
                username = model.Username.Trim();
            }
            string email = string.Empty;
            if (model.Email != null)
            {
                email = model.Email.Trim();
            }

            if (this.db.Users.Any(u => u.Username == username))
            {
                this.ModelState.AddModelError("Username", "Tên đăng nhập này đã được sử dụng.");
            }
            if (this.db.Users.Any(u => u.Email == email))
            {
                this.ModelState.AddModelError("Email", "Email này đã được sử dụng.");
            }

            if (!this.ModelState.IsValid)
            {
                model.RoleList = this.BuildRoleList();
                return this.View(model);
            }

            AppUser account = new AppUser();
            account.Username = username;
            account.FullName = model.FullName.Trim();
            account.Email = email;
            account.Role = model.Role;
            account.IsActive = model.IsActive;
            account.PasswordSalt = PasswordHelper.GenerateSalt();
            account.PasswordHash = PasswordHelper.HashPassword(model.Password, account.PasswordSalt);
            account.CreatedAt = DateTime.Now;

            this.db.Users.Add(account);
            this.db.SaveChanges();

            this.TempData["Message"] = "Đã thêm tài khoản " + account.Username + ".";
            return this.RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            AppUser account = this.db.Users.Find(id);
            if (account == null)
            {
                return this.HttpNotFound();
            }

            UserFormViewModel model = new UserFormViewModel();
            model.UserId = account.UserId;
            model.Username = account.Username;
            model.FullName = account.FullName;
            model.Email = account.Email;
            model.Role = account.Role;
            model.IsActive = account.IsActive;
            model.RoleList = this.BuildRoleList();
            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(UserFormViewModel model)
        {
            AppUser account = this.db.Users.Find(model.UserId);
            if (account == null)
            {
                return this.HttpNotFound();
            }

            // Tên đăng nhập không được đổi: bỏ qua giá trị gửi lên, dùng giá trị trong cơ sở dữ liệu
            model.Username = account.Username;
            this.ModelState.Remove("Username");

            if (model.Role != "Admin" && model.Role != "Member")
            {
                this.ModelState.AddModelError("Role", "Vai trò không hợp lệ.");
            }

            // Không cho Admin tự hạ quyền hoặc tự khóa chính mình, tránh việc không còn ai quản trị được
            if (account.UserId == this.CurrentUser.UserId && (model.Role != "Admin" || !model.IsActive))
            {
                this.ModelState.AddModelError("", "Bạn không thể tự hạ quyền hoặc tự khóa tài khoản của chính mình.");
            }

            string email = string.Empty;
            if (model.Email != null)
            {
                email = model.Email.Trim();
            }
            int userId = account.UserId;
            if (this.db.Users.Any(u => u.Email == email && u.UserId != userId))
            {
                this.ModelState.AddModelError("Email", "Email này đã được sử dụng.");
            }

            if (!this.ModelState.IsValid)
            {
                model.RoleList = this.BuildRoleList();
                return this.View(model);
            }

            account.FullName = model.FullName.Trim();
            account.Email = email;
            account.Role = model.Role;
            account.IsActive = model.IsActive;

            // Chỉ đổi mật khẩu khi Admin nhập mật khẩu mới
            if (!string.IsNullOrWhiteSpace(model.Password))
            {
                account.PasswordSalt = PasswordHelper.GenerateSalt();
                account.PasswordHash = PasswordHelper.HashPassword(model.Password, account.PasswordSalt);
            }

            this.db.SaveChanges();
            this.TempData["Message"] = "Đã cập nhật tài khoản " + account.Username + ".";
            return this.RedirectToAction("Index");
        }

        private List<SelectListItem> BuildRoleList()
        {
            List<SelectListItem> list = new List<SelectListItem>();
            list.Add(new SelectListItem { Value = "Member", Text = "Member (thành viên)" });
            list.Add(new SelectListItem { Value = "Admin", Text = "Admin (quản trị viên)" });
            return list;
        }
    }
}
