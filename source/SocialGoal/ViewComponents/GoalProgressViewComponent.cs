
using Microsoft.AspNetCore.Mvc;
using SocialGoal.Service;
using System.Threading.Tasks;

namespace SocialGoal.Web.ViewComponents
{
    public class GoalProgressViewComponent : ViewComponent
    {
        private readonly IGroupUpdateService _groupUpdateService;

        public GoalProgressViewComponent(IGroupUpdateService groupUpdateService)
        {
            _groupUpdateService = groupUpdateService;
        }

        public async Task<IViewComponentResult> InvokeAsync(int id)
        {
            double progress = _groupUpdateService.Progress(id);
            return View(progress);
        }
    }
}