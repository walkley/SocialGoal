using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using PagedList.Core;

namespace SocialGoal.Web.Core.AutoMapperConverters
{
    public class PagedListConverter<T1, T2> : ITypeConverter<IPagedList<T1>, IPagedList<T2>>
    {
        public IPagedList<T2> Convert(IPagedList<T1> source, IPagedList<T2> destination, ResolutionContext context)
        {
            var models = source;
            var viewModels = models.Select(x => context.Mapper.Map<T2>(x));
            return new StaticPagedList<T2>(viewModels, models.PageNumber, models.PageSize, models.TotalItemCount);
        }
    }
}
