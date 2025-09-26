
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SocialGoal.Service;
using System.Linq;
using System.Threading.Tasks;

namespace SocialGoal.Web.ViewComponents
{
    public class GetNumberOfInvitationsViewComponent : ViewComponent
    {
        private readonly IGroupInvitationService _groupInvitationService;
        private readonly ISupportInvitationService _supportInvitationService;
        private readonly IFollowRequestService _followRequestService;
        private readonly UserManager<IdentityUser> _userManager;

        public GetNumberOfInvitationsViewComponent(
            IGroupInvitationService groupInvitationService,
            ISupportInvitationService supportInvitationService,
            IFollowRequestService followRequestService,
            UserManager<IdentityUser> userManager)
        {
            _groupInvitationService = groupInvitationService;
            _supportInvitationService = supportInvitationService;
            _followRequestService = followRequestService;
            _userManager = userManager;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var userId = _userManager.GetUserId(HttpContext.User);
            var count = _groupInvitationService.GetGroupInvitationsForUser(userId).Count() +
                       _supportInvitationService.GetSupportInvitationsForUser(userId).Count() +
                       _followRequestService.GetFollowRequestsForUser(userId).Count();

            return Content(count.ToString());
        }
    }
}
