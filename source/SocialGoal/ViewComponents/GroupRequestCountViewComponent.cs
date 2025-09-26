
using Microsoft.AspNetCore.Mvc;
using SocialGoal.Service;
using System.Linq;
using System.Threading.Tasks;

namespace SocialGoal.Web.ViewComponents
{
    public class GroupRequestCountViewComponent : ViewComponent
    {
        private readonly IGroupRequestService groupRequestService;

        public GroupRequestCountViewComponent(IGroupRequestService groupRequestService)
        {
            this.groupRequestService = groupRequestService;
        }

        public Task<IViewComponentResult> InvokeAsync(int id)
        {
            int count = groupRequestService.GetGroupRequestsForGroup(id).Count();
            return Task.FromResult<IViewComponentResult>(Content(count.ToString()));
        }
    }
}
