using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.Rendering;


namespace SocialGoal.Web.Core.Helpers
{
    public class UserHtmlHelper: BaseHtmlHelper
    {
        public UserHtmlHelper(IHtmlHelper html, IUrlHelper url) : base(html,url)
        {
        }

        public HtmlString Avatar(string profilePicUrl, object htmlAttributes=null)
        {
            var src = string.IsNullOrEmpty(profilePicUrl) ?
                "../../Content/templatemo_329_blue_urban/images/facebook-avatar.png"
                : Url.Content(profilePicUrl);
            var tag = new TagBuilder("img");
            tag.MergeAttributes(HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes), true);
            tag.AddCssClass("thumbnail");
            tag.Attributes.Add("src", src);
            return new HtmlString(tag.ToString());
        }
    }
}
