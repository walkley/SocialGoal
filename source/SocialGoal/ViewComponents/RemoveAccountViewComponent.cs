
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SocialGoal.Models;
using System.Threading.Tasks;
using System.Security.Claims;
using SocialGoal.Model.Models;

namespace SocialGoal.ViewComponents
{
    public class RemoveAccountViewComponent : ViewComponent
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public RemoveAccountViewComponent(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var userId = ((ClaimsPrincipal)User).FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId);
            var linkedAccounts = await _userManager.GetLoginsAsync(user);
            ViewBag.ShowRemoveButton = HasPassword(userId) || linkedAccounts.Count > 1;
            return View("Default", linkedAccounts);
        }

        private bool HasPassword(string userId)
        {
            var user = _userManager.FindByIdAsync(userId).Result;
            if (user != null)
            {
                return user.PasswordHash != null;
            }
            return false;
        }
    }
}
