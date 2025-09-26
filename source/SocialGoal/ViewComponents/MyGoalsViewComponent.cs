
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SocialGoal.Service;
using AutoMapper;
using System.Collections.Generic;
using SocialGoal.Model.Models;
using SocialGoal.Web.ViewModels;
using Microsoft.AspNetCore.Identity;

namespace SocialGoal.Web.ViewComponents
{
    public class MyGoalsViewComponent : ViewComponent
    {
        private readonly IGoalService goalService;

        public MyGoalsViewComponent(IGoalService goalService)
        {
            this.goalService = goalService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            string userid = UserClaimsPrincipal.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var Goals = goalService.GetMyGoals(userid);
            return View("_MyGoalsView", Goals);
        }
    }
}