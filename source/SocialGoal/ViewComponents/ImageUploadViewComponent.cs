
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using SocialGoal.Web.ViewModels;
using SocialGoal.Service;
using System.Threading.Tasks;
using SocialGoal.Model.Models;

namespace SocialGoal.Web.ViewComponents
{
    public class ImageUploadViewComponent : ViewComponent
    {
        private readonly IUserService _userService;
        private readonly UserManager<ApplicationUser> _userManager;

        public ImageUploadViewComponent(IUserService userService, UserManager<ApplicationUser> userManager)
        {
            _userService = userService;
            _userManager = userManager;
        }

        public IViewComponentResult Invoke()
        {
            var userId = _userManager.GetUserId(HttpContext.User);
            UploadImageViewModel imageVM = new UploadImageViewModel()
            {
                LocalPath = _userService.GetUser(userId).ProfilePicUrl
            };
            return View(imageVM);
        }
    }
}
