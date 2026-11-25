using System.Web.Mvc;

namespace TaskBoard.Helpers
{
    // Giống [Authorize] nhưng phân biệt hai trường hợp:
    // 1. Chưa đăng nhập: chuyển tới trang đăng nhập.
    // 2. Đã đăng nhập nhưng không đủ quyền: trả về trang "Không có quyền" với mã 403
    //    (nếu dùng [Authorize] thường thì người dùng bị đẩy về trang đăng nhập lặp lại).
    public class RoleAuthorizeAttribute : AuthorizeAttribute
    {
        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            if (filterContext.HttpContext.User.Identity.IsAuthenticated)
            {
                filterContext.HttpContext.Response.StatusCode = 403;
                filterContext.HttpContext.Response.TrySkipIisCustomErrors = true;
                ViewResult result = new ViewResult();
                result.ViewName = "Forbidden";
                filterContext.Result = result;
            }
            else
            {
                base.HandleUnauthorizedRequest(filterContext);
            }
        }
    }
}
