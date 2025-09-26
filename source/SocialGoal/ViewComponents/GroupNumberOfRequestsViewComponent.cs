
using Microsoft.AspNetCore.Mvc;
using SocialGoal.Service;
using System.Linq;
using System.Threading.Tasks;

namespace SocialGoal.Web.ViewComponents
{
    public class GroupNumberOfRequestsViewComponent : ViewComponent
    {
        private readonly IGroupRequestService groupRequestService;

        public GroupNumberOfRequestsViewComponent(IGroupRequestService groupRequestService)
        {
            this.groupRequestService = groupRequestService;
        }

        public async Task<IViewComponentResult> InvokeAsync(int id)
        {
            // Get the number of requests for the group
            var numberOfRequests = groupRequestService.GetGroupRequestsForGroup(id).Count();

            // Return the count as the result
            return View(numberOfRequests);
        }
    }
}
