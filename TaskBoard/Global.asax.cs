using System;
using System.Security.Claims;
using System.Security.Principal;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;

namespace TaskBoard
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            // Identity tự tạo (GenericIdentity) chỉ có claim Name. Nếu không khai báo dòng này,
            // [ValidateAntiForgeryToken] báo lỗi thiếu claim NameIdentifier khi người dùng đã đăng nhập.
            AntiForgeryConfig.UniqueClaimTypeIdentifier = ClaimTypes.Name;

            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        // Sau khi Forms Authentication đọc cookie, đọc vai trò (Admin hoặc Member) lưu trong ticket
        // và gắn vào User để [RoleAuthorize(Roles = "Admin")] và User.IsInRole() hoạt động.
        protected void Application_PostAuthenticateRequest(object sender, EventArgs e)
        {
            HttpCookie cookie = Request.Cookies[FormsAuthentication.FormsCookieName];
            if (cookie == null)
            {
                return;
            }

            try
            {
                FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(cookie.Value);
                if (ticket == null || ticket.Expired)
                {
                    return;
                }

                GenericIdentity identity = new GenericIdentity(ticket.Name, "Forms");
                GenericPrincipal principal = new GenericPrincipal(identity, new string[] { ticket.UserData });
                HttpContext.Current.User = principal;
            }
            catch (Exception)
            {
                // Cookie hỏng hoặc bị sửa: bỏ qua, người dùng coi như chưa đăng nhập
                return;
            }
        }
    }
}
