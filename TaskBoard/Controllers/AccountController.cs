using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using TaskBoard.Helpers;
using TaskBoard.Models;
using TaskBoard.Models.ViewModels;

namespace TaskBoard.Controllers
{
    public class AccountController : BaseController
    {
        [HttpGet]
        public ActionResult Login(string returnUrl)
        {
            if (this.User.Identity.IsAuthenticated)
            {
                return this.RedirectToAction("Index", "Projects");
            }
            this.ViewBag.ReturnUrl = returnUrl;
            return this.View(new LoginViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginViewModel model, string returnUrl)
        {
            this.ViewBag.ReturnUrl = returnUrl;
            if (!this.ModelState.IsValid)
            {
                return this.View(model);
            }

            string username = model.Username.Trim();
            AppUser account = this.db.Users.FirstOrDefault(u => u.Username == username);

            // Một thông báo chung cho mọi lỗi để không lộ tên đăng nhập nào tồn tại
            bool success = account != null
                && account.IsActive
                && PasswordHelper.Verify(model.Password, account.PasswordSalt, account.PasswordHash);

            if (!success)
            {
                this.ModelState.AddModelError("", "Tên đăng nhập hoặc mật khẩu không đúng, hoặc tài khoản đã bị khóa.");
                return this.View(model);
            }

            this.SignIn(account, model.RememberMe);

            // Chỉ chuyển tới địa chỉ nội bộ để tránh bị lợi dụng chuyển hướng sang trang khác
            if (!string.IsNullOrEmpty(returnUrl) && this.Url.IsLocalUrl(returnUrl))
            {
                return this.Redirect(returnUrl);
            }
            return this.RedirectToAction("Index", "Projects");
        }

        [HttpGet]
        public ActionResult Register()
        {
            if (this.User.Identity.IsAuthenticated)
            {
                return this.RedirectToAction("Index", "Projects");
            }
            return this.View(new RegisterViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterViewModel model)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(model);
            }

            string username = model.Username.Trim();
            string email = model.Email.Trim();

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
                return this.View(model);
            }

            AppUser account = new AppUser();
            account.Username = username;
            account.FullName = model.FullName.Trim();
            account.Email = email;
            account.PasswordSalt = PasswordHelper.GenerateSalt();
            account.PasswordHash = PasswordHelper.HashPassword(model.Password, account.PasswordSalt);
            account.Role = "Member";   // Người tự đăng ký luôn là Member, chỉ Admin mới tạo được Admin khác
            account.IsActive = true;
            account.CreatedAt = DateTime.Now;

            this.db.Users.Add(account);
            this.db.SaveChanges();

            this.SignIn(account, false);
            this.TempData["Message"] = "Đăng ký thành công. Chào mừng bạn đến với TaskBoard.";
            return this.RedirectToAction("Index", "Projects");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return this.RedirectToAction("Index", "Home");
        }

        // Tạo cookie đăng nhập. Vai trò được lưu trong phần UserData của ticket đã mã hóa.
        private void SignIn(AppUser account, bool rememberMe)
        {
            DateTime now = DateTime.Now;
            DateTime expires = now.AddHours(2);
            if (rememberMe)
            {
                expires = now.AddDays(7);
            }

            FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(
                1, account.Username, now, expires, rememberMe, account.Role);
            string encryptedTicket = FormsAuthentication.Encrypt(ticket);

            HttpCookie cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket);
            cookie.HttpOnly = true;
            if (rememberMe)
            {
                cookie.Expires = expires;
            }
            this.Response.Cookies.Add(cookie);
        }
    }
}
