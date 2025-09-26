using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;


namespace SocialGoal.Web.Core.Helpers
{
    public class BaseHtmlHelper
    {
        private readonly IHtmlHelper _html;
        private readonly IUrlHelper _url;

        public BaseHtmlHelper(IHtmlHelper html, IUrlHelper url)
        {
            _html = html;
            _url = url;
        }
        protected IHtmlHelper Html { get { return _html; } }
        protected IUrlHelper Url { get { return _url; } }
    }
}
