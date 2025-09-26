
using Microsoft.AspNetCore.Mvc;
using SocialGoal.Service;
using SocialGoal.Web.ViewModels;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;

namespace SocialGoal.Web.ViewModels
{
    public class UserDetailViewModel
    {
        public string UserId { get; set; }
        // Add other properties as needed
    }
}

namespace SocialGoal.Web.ViewComponents
{
    public class UserDetailViewComponent : ViewComponent
    {
        private readonly IUserService _userService;
        private readonly IUserProfileService _userProfileService;

        public UserDetailViewComponent(IUserService userService, IUserProfileService userProfileService)
        {
            _userService = userService;
            _userProfileService = userProfileService;
        }

        public async Task<IViewComponentResult> InvokeAsync(string id)
        {
            // This is a placeholder for the actual implementation
            // You'll need to implement the same logic that was in the UserDetail action
            var user = _userService.GetUserProfile(id);
            var userdetail = _userProfileService.GetUser(id);

            // Create your view model with the data
            var model = new UserDetailViewModel
            {
                // Map properties from user and userdetail to your view model
                // This is just an example, adjust according to your actual model
                UserId = id,
                // Add other properties as needed
            };

            return View(model);
        }
    }
}
