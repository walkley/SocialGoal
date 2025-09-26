using AutoMapper;
using SocialGoal.Model.Models;
using SocialGoal.Web.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SocialGoal.Mappings
{

    public class ViewModelToDomainMappingProfile : Profile
    {
        public ViewModelToDomainMappingProfile()
        {
            CreateMap<CommentFormModel, Comment>();
            CreateMap<GroupFormModel, Group>();
            CreateMap<FocusFormModel, Focus>();
            CreateMap<UpdateFormModel, Update>();
            CreateMap<UserFormModel, ApplicationUser>();
            CreateMap<UserProfileFormModel, UserProfile>();
            CreateMap<GroupGoalFormModel, GroupGoal>();
            CreateMap<GroupUpdateFormModel, GroupUpdate>();
            CreateMap<GroupCommentFormModel, GroupComment>();
            CreateMap<GroupRequestFormModel, GroupRequest>();
            CreateMap<FollowRequestFormModel, FollowRequest>();
            CreateMap<GoalFormModel, Goal>();
            //CreateMap<XViewModel, X>()
            //    .ForMember(x => x.PropertyXYZ, opt => opt.MapFrom(source => source.Property1));
        }
    }
}
