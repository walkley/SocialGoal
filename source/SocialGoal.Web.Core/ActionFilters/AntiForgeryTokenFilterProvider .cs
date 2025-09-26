using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace SocialGoal.Web.Core.ActionFilters
{
    public class AntiForgeryTokenFilterProvider : IFilterProvider
    {
        public void OnProvidersExecuting(FilterProviderContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var httpMethod = context.ActionContext.HttpContext.Request.Method;

            if (String.Equals(httpMethod, "POST", StringComparison.OrdinalIgnoreCase))
            {
                context.Results.Add(new FilterItem(new FilterDescriptor(new ValidateAntiForgeryTokenAttribute(), FilterScope.Global)));
            }
        }

        public void OnProvidersExecuted(FilterProviderContext context)
        {
            // No implementation needed
        }

        public int Order => 0;
    }
}
