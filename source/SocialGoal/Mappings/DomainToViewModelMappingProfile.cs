using AutoMapper;
using PagedList;
using SocialGoal.Model.Models;
using SocialGoal.Web.Core.AutoMapperConverters;
using SocialGoal.Web.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SocialGoal.Mappings
{
    public class DomainToViewModelMappingProfile : Profile
    {
        public DomainToViewModelMappingProfile()
        {
            CreateMap<Goal, GoalViewModel>();
            CreateMap<Goal, GoalFormModel>();
            CreateMap<Comment, CommentsViewModel>();
            CreateMap<UserProfile, UserProfileFormModel>();
            CreateMap<Group, GroupGoalFormModel>();
            CreateMap<Group, GroupFormModel>();
            CreateMap<GroupGoal, GroupGoalFormModel>();
            CreateMap<GroupInvitation, NotificationViewModel>();
            CreateMap<SupportInvitation, NotificationViewModel>();
            CreateMap<Group, GroupViewModel>();
            CreateMap<GroupGoal, GroupGoalViewModel>();
            CreateMap<GroupComment, GroupCommentsViewModel>();
            CreateMap<Focus, FocusViewModel>();
            CreateMap<Focus, FocusFormModel>();
            CreateMap<GroupRequest, GroupRequestViewModel>();
            CreateMap<FollowRequest, NotificationViewModel>();
            CreateMap<ApplicationUser, FollowersViewModel>();
            CreateMap<ApplicationUser, FollowingViewModel>();
            CreateMap<Update, UpdateFormModel>();
            CreateMap<GroupUpdate, GroupUpdateFormModel>();
            CreateMap<Update, UpdateViewModel>();
            CreateMap<GroupUpdate, GroupUpdateViewModel>();
            //CreateMap<X, XViewModel>()
            //    .ForMember(x => x.Property1, opt => opt.MapFrom(source => source.PropertyXYZ));
            CreateMap<Goal, GoalListViewModel>()
                .ForMember(x => x.SupportsCount, opt => opt.MapFrom(source => source.Supports.Count))
                .ForMember(x => x.UserName, opt => opt.MapFrom(source => source.User.UserName))
                .ForMember(x => x.StartDate, opt => opt.MapFrom(source => source.StartDate.ToString("dd MMM yyyy")))
                .ForMember(x => x.EndDate, opt => opt.MapFrom(source => source.EndDate.ToString("dd MMM yyyy")));

            CreateMap<Group, GroupsItemViewModel>()
                .ForMember(x => x.CreatedDate, opt => opt.MapFrom(source => source.CreatedDate.ToString("dd MMM yyyy")));

            // Use extension method for PagedListConverter to ensure compatibility with AutoMapper 13.0
            CreateMap(typeof(IPagedList<>), typeof(IPagedList<>))
                .ConvertUsing(typeof(PagedListConverter<,>));
        }
    }
}
