using SocialGoal.Service;
using SocialGoal.Web.Core.Models;
using SocialGoal.Web.Services;
using SocialGoal.Web.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using SocialGoal.Model.Models;


namespace SocialGoal.Controllers
{
    public static class RequestExtensions
    {
        public static bool IsAjaxRequest(this Microsoft.AspNetCore.Http.HttpRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            return request.Headers["X-Requested-With"] == "XMLHttpRequest";
        }
    }
    [Authorize]
    public class HomeController : Controller
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
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly CreateNotificationList notificationListCreation = new CreateNotificationList();

        public HomeController(IMetricService metricService, IFocusService focusService, IGoalService goalService, ICommentService commentService, IUpdateService updateService, ISupportService supportService, IUserService userService, IGroupUserService groupUserService, IGroupService groupService, IGroupGoalService groupGoalService, IGroupUpdateService groupupdateService, IGroupCommentService groupcommentService, IFollowUserService followUserService, IGroupUpdateUserService groupUpdateUserService, IGroupCommentUserService groupCommentUserService, ICommentUserService commentUserService, UserManager<ApplicationUser> userManager)
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
            this._userManager = userManager;
        }

        /// <summary>
        /// returns all notifications on first load
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        public ActionResult Index(int page = 0)
        {
            int noOfRecords = 10;
            HomeViewModel dashboard = new HomeViewModel()
            {
                Notification = GetNotifications(page, noOfRecords),
                Count = GetNotifications(page, noOfRecords).Count()
            };

            if (RequestExtensions.IsAjaxRequest(Request))
            {
                if (dashboard.Count != 0)
                    return PartialView("Notification", dashboard);
                else
                    return null;
            }
            return View(dashboard);
        }




        public ViewResult About()
        {
            //ViewBag.Message = "Your quintessential app description page.";

            return View();
        }

        public ViewResult Contact()
        {
            //ViewBag.Message = "Your quintessential contact page.";

            return View();
        }







        public PartialViewResult UserNotification(string id)
        {
            HomeViewModel dashboard = new HomeViewModel()
            {
                Notification = GetNotifications(id)
            };
            return PartialView("Notification", dashboard);
        }


        /// <summary>
        /// get notifications paged
        /// </summary>
        /// <param name="?"></param>
        /// <param name="?"></param>
        /// <returns></returns>
        public IEnumerable<NotificationsViewModel> GetNotifications(int page, int noOfRecords)
        {
            var notifications = notificationListCreation.GetNotifications(_userManager.GetUserId(User), goalService, commentService, updateService, supportService, userService, groupService, groupUserService, groupGoalService, groupcommentService, groupupdateService, followUserService, groupCommentUserService, commentUserService, groupUpdateUserService);

            var skipNotifications = noOfRecords * page;
            notifications = notifications.Skip(skipNotifications).Take(noOfRecords);

            return notifications;
        }

        public IEnumerable<NotificationsViewModel> GetNotifications(string userid)
        {

            var notifications = notificationListCreation.GetProfileNotifications(userid, goalService, commentService, updateService, supportService, userService, groupService, groupUserService, groupGoalService, groupcommentService, groupupdateService, commentUserService);
            return notifications;
        }
    }
}
