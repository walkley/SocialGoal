using AutoMapper;
using SocialGoal.Model.Models;
using SocialGoal.Service;
using SocialGoal.Web.Core.Models;
using SocialGoal.Web.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace SocialGoal.Web.Controllers
{
    // Extension method for AJAX request detection
    public static class HttpRequestExtensions
    {
        public static bool IsAjaxRequest(this Microsoft.AspNetCore.Http.HttpRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            return request.Headers["X-Requested-With"] == "XMLHttpRequest";
        }
    }


    [Authorize]
    public class NotificationController : Controller
    {
        IGoalService goalService;
        IUpdateService updateService;
        ICommentService commentService;
        IGroupInvitationService groupInvitationService;
        ISupportInvitationService supportInvitationService;
        IFollowRequestService followRequestService;
        IUserService userService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;
        public NotificationController(IGoalService goalService, IUpdateService updateService, ICommentService commentService, IGroupInvitationService groupInvitationService, ISupportInvitationService supportInvitationService, IFollowRequestService followRequestService, IUserService userService, UserManager<ApplicationUser> userManager, IMapper mapper)
        {
            this.goalService = goalService;
            this.supportInvitationService = supportInvitationService;
            this.updateService = updateService;
            this.groupInvitationService = groupInvitationService;
            this.commentService = commentService;
            this.followRequestService = followRequestService;
            this.userService = userService;
            _userManager = userManager;
            _mapper = mapper;
        }


        /// <summary>
        /// Action that returns invitations
        /// </summary>
        /// <param name="page">current page no will be bere</param>
        /// <returns></returns>
        public ActionResult Index(int page = 0)
        {
            int noOfrecords = 10;
            var notifications = GetNotifications(page, noOfrecords);
            if (Request.IsAjaxRequest())
            {
                return PartialView("_NotificationList", notifications);
            }
            return View("Index", notifications);
        }



        public int GetNumberOfInvitations()
        {
            return groupInvitationService.GetGroupInvitationsForUser(_userManager.GetUserId(User)).Count() + supportInvitationService.GetSupportInvitationsForUser(_userManager.GetUserId(User)).Count() + followRequestService.GetFollowRequestsForUser(_userManager.GetUserId(User)).Count();
        }

        /// <summary>
        /// Method returns paged notifications
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        public IEnumerable<NotificationViewModel> GetNotifications(int page, int noOfRecords)
        {
            var groupInv = groupInvitationService.GetGroupInvitationsForUser(_userManager.GetUserId(User));
            var supportInv = supportInvitationService.GetSupportInvitationsForUser(_userManager.GetUserId(User));
            var followRequests = followRequestService.GetFollowRequests(_userManager.GetUserId(User));
            IEnumerable<NotificationViewModel> Invitations = _mapper.Map<IEnumerable<GroupInvitation>, IEnumerable<NotificationViewModel>>(groupInv);
            Invitations = Invitations.Concat(_mapper.Map<IEnumerable<SupportInvitation>, IEnumerable<NotificationViewModel>>(supportInv));
            Invitations = Invitations.Concat(_mapper.Map<IEnumerable<FollowRequest>, IEnumerable<NotificationViewModel>>(followRequests));
            foreach (var item in Invitations)
            {
                var fromUser = userService.GetUser(item.FromUserId);
                var toUser = userService.GetUser(item.ToUserId);
                item.FromUser = fromUser;
                item.ToUser = toUser;
            }

            //for paging

            var skipNotifications = noOfRecords * page;
            Invitations = Invitations.Skip(skipNotifications).Take(noOfRecords);
            return Invitations;
        }
    }
}
