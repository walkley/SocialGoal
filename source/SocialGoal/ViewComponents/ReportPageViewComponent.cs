
using Microsoft.AspNetCore.Mvc;

namespace SocialGoal.Web.ViewComponents
{
    public class ReportPageViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(int id)
        {
            return View("Default");
        }
    }
}