
using Microsoft.AspNetCore.Mvc;
using SocialGoal.Service;
using System.Threading.Tasks;

namespace SocialGoal.Web.ViewComponents
{
    public class InviteUserViewComponent : ViewComponent
    {
        private readonly IGoalService goalService;

        public InviteUserViewComponent(IGoalService goalService)
        {
            this.goalService = goalService;
        }

        public async Task<IViewComponentResult> InvokeAsync(int id)
        {
            var goal = goalService.GetGoal(id);
            return View(goal);
        }
    }
}