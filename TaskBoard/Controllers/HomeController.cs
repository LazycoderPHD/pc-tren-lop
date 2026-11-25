using System.Web.Mvc;

namespace TaskBoard.Controllers
{
    public class HomeController : BaseController
    {
        public ActionResult Index()
        {
            // Người đã đăng nhập vào thẳng danh sách dự án
            if (this.User.Identity.IsAuthenticated)
            {
                return this.RedirectToAction("Index", "Projects");
            }
            return this.View();
        }

        public ActionResult Privacy()
        {
            return this.View();
        }

        public ActionResult Terms()
        {
            return this.View();
        }
    }
}
