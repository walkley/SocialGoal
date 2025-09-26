
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using SocialGoal.Service;
using SocialGoal.Web.Core.Models;
using SocialGoal.Web.Services;
using SocialGoal.Web.ViewModels;
using System.Linq;
using Microsoft.AspNetCore.Identity;

namespace SocialGoal.Web.ViewComponents
{
    public class UserNotificationViewComponent : ViewComponent
    {
        IMetricService metricService;
        IFocusService focusService;
        IGoalService goalService;
        ICommentService commentService;
        IUpdateService updateService;
        ISupportService supportService;
        IUserService userService;
        IGroupService groupService;
        IGroupUserService groupUserService;
        IGroupGoalService groupGoalService;
        IGroupUpdateService groupupdateService;
        IGroupCommentService groupcommentService;
        IFollowUserService followUserService;
        ICommentUserService commentUserService;
        IGroupCommentUserService groupCommentUserService;
        IGroupUpdateUserService groupUpdateUserService;
        private readonly CreateNotificationList notificationListCreation = new CreateNotificationList();

        public UserNotificationViewComponent(IMetricService metricService, IFocusService focusService, IGoalService goalService, 
            ICommentService commentService, IUpdateService updateService, ISupportService supportService, 
            IUserService userService, IGroupUserService groupUserService, IGroupService groupService, 
            IGroupGoalService groupGoalService, IGroupUpdateService groupupdateService, 
            IGroupCommentService groupcommentService, IFollowUserService followUserService, 
            IGroupUpdateUserService groupUpdateUserService, IGroupCommentUserService groupCommentUserService, 
            ICommentUserService commentUserService)
        {
            this.metricService = metricService;
            this.focusService = focusService;
            this.goalService = goalService;
            this.commentService = commentService;
            this.updateService = updateService;
            this.supportService = supportService;
            this.userService = userService;
            this.groupService = groupService;
            this.groupUserService = groupUserService;
            this.groupGoalService = groupGoalService;
            this.groupupdateService = groupupdateService;
            this.groupcommentService = groupcommentService;
            this.followUserService = followUserService;
            this.groupCommentUserService = groupCommentUserService;
            this.groupUpdateUserService = groupUpdateUserService;
            this.commentUserService = commentUserService;
        }

        public IViewComponentResult Invoke(string id)
        {
            HomeViewModel dashboard = new HomeViewModel()
            {
                Notification = GetNotifications(id)
            };
            return View(dashboard);
        }

        public IEnumerable<NotificationsViewModel> GetNotifications(string userid)
        {
            var notifications = notificationListCreation.GetProfileNotifications(userid, goalService, commentService, updateService, 
                supportService, userService, groupService, groupUserService, groupGoalService, groupcommentService, 
                groupupdateService, commentUserService);
            return notifications;
        }
    }
}