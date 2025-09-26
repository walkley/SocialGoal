
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SocialGoal.Service;
using AutoMapper;
using System.Collections.Generic;
using SocialGoal.Model.Models;
using SocialGoal.Web.ViewModels;
using Microsoft.AspNetCore.Identity;
using System.Linq;

namespace SocialGoal.Web.ViewComponents
{
    public class FollowedGroupsViewComponent : ViewComponent
    {
        private readonly IGroupService groupService;
        private readonly IGroupUserService groupUserService;

        public FollowedGroupsViewComponent(IGroupService groupService, IGroupUserService groupUserService)
        {
            this.groupService = groupService;
            this.groupUserService = groupUserService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            string userId = UserClaimsPrincipal.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            List<Group> groups = new List<Group> { };
            var groupids = groupUserService.GetFollowedGroups(userId);
            foreach (var item in groupids)
            {
                var group = groupService.GetGroup(item);
                groups.Add(group);
            }
            return View("FollowedGroups", groups);
        }
    }
}