
using Microsoft.AspNetCore.Mvc;
using SocialGoal.Service;
using System.Threading.Tasks;

namespace SocialGoal.Web.ViewComponents
{
    public class NoOfSupportsViewComponent : ViewComponent
    {
        private readonly IGroupUpdateSupportService _groupUpdateSupportService;

        public NoOfSupportsViewComponent(IGroupUpdateSupportService groupUpdateSupportService)
        {
            _groupUpdateSupportService = groupUpdateSupportService;
        }

        public async Task<IViewComponentResult> InvokeAsync(int id)
        {
            int supportCount = _groupUpdateSupportService.GetSupportcount(id);
            return View(supportCount);
        }
    }
}