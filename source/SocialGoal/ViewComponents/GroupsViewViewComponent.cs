
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
    public class GroupsViewViewComponent : ViewComponent
    {
        private readonly IGroupService groupService;
        private readonly IGroupUserService groupUserService;
        private readonly IMapper mapper;

        public GroupsViewViewComponent(IGroupService groupService, IGroupUserService groupUserService, IMapper mapper)
        {
            this.groupService = groupService;
            this.groupUserService = groupUserService;
            this.mapper = mapper;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            string userId = UserClaimsPrincipal.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var groupIds = groupUserService.GetGroupAdminUsers(userId);
            var groups = groupService.GetGroupsForUser(groupIds);
            var groupsList = mapper.Map<IEnumerable<Group>, IEnumerable<GroupsItemViewModel>>(groups);
            return View("_GroupView", groupsList);
        }
    }
}
