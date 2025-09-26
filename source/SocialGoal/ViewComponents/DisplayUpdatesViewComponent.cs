
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using SocialGoal.Model.Models;
using SocialGoal.Service;
using SocialGoal.Web.ViewModels;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace SocialGoal.Web.ViewComponents
{
    public class DisplayUpdatesViewComponent : ViewComponent
    {
        private readonly IGroupUpdateService groupUpdateService;
        private readonly IGroupGoalService groupGoalService;
        private readonly IGroupUpdateSupportService groupUpdateSupportService;
        private readonly IGroupUpdateUserService groupUpdateUserService;
        private readonly IGroupUserService groupUserService;
        private readonly IMapper mapper;

        public DisplayUpdatesViewComponent(
            IGroupUpdateService groupUpdateService,
            IGroupGoalService groupGoalService,
            IGroupUpdateSupportService groupUpdateSupportService,
            IGroupUpdateUserService groupUpdateUserService,
            IGroupUserService groupUserService,
            IMapper mapper)
        {
            this.groupUpdateService = groupUpdateService;
            this.groupGoalService = groupGoalService;
            this.groupUpdateSupportService = groupUpdateSupportService;
            this.groupUpdateUserService = groupUpdateUserService;
            this.groupUserService = groupUserService;
            this.mapper = mapper;
        }

        public IViewComponentResult Invoke(int id)
        {
            var Updates = mapper.Map<IEnumerable<GroupUpdate>, IEnumerable<GroupUpdateViewModel>>(groupUpdateService.GetUpdatesByGoal(id));

            foreach (var item in Updates)
            {
                item.IsSupported = groupUpdateSupportService.IsUpdateSupported(item.GroupUpdateId, ((ClaimsPrincipal)User).FindFirstValue(ClaimTypes.NameIdentifier), groupUserService);
                item.UserId = groupUpdateUserService.GetGroupUpdateUser(item.GroupUpdateId).Id;
            }

            GroupUpdateListViewModel updates = new GroupUpdateListViewModel()
            {
                GroupUpdates = Updates,
                Metric = groupGoalService.GetGroupGoal(id).Metric,
                Target = groupGoalService.GetGroupGoal(id).Target
            };

            return View("Default", updates);
        }
    }
}
