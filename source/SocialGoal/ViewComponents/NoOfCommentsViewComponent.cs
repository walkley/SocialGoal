
using Microsoft.AspNetCore.Mvc;
using SocialGoal.Service;
using System.Threading.Tasks;

namespace SocialGoal.Web.ViewComponents
{
    public class NoOfCommentsViewComponent : ViewComponent
    {
        private readonly IGroupCommentService _groupCommentService;

        public NoOfCommentsViewComponent(IGroupCommentService groupCommentService)
        {
            _groupCommentService = groupCommentService;
        }

        public async Task<IViewComponentResult> InvokeAsync(int id)
        {
            int commentCount = _groupCommentService.GetCommentcount(id);
            return View(commentCount);
        }
    }
}