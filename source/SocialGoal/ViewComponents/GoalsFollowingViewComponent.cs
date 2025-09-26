
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
    public class GoalsFollowingViewComponent : ViewComponent
    {
        private readonly ISupportService supportService;
        private readonly IGoalService goalService;

        public GoalsFollowingViewComponent(ISupportService supportService, IGoalService goalService)
        {
            this.supportService = supportService;
            this.goalService = goalService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            string userId = UserClaimsPrincipal.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var followed = supportService.GetUserSupportedGoals(userId, goalService);
            return View("_FollowedGoals", followed);
        }
    }
}