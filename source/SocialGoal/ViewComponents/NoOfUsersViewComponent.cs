
using Microsoft.AspNetCore.Mvc;
using SocialGoal.Service;
using System.Threading.Tasks;

namespace SocialGoal.Web.ViewComponents
{
    public class NoOfUsersViewComponent : ViewComponent
    {
        private readonly IGroupUserService _groupUserService;

        public NoOfUsersViewComponent(IGroupUserService groupUserService)
        {
            _groupUserService = groupUserService;
        }

        public async Task<IViewComponentResult> InvokeAsync(int id)
        {
            // Get the count of users in the group
            int userCount = _groupUserService.GetGroupUsersCount(id);
            
            // Return the count directly as content
            return Content(userCount.ToString());
        }
    }
}