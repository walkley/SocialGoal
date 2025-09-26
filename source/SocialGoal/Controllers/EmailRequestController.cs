using SocialGoal.Model.Models;
using SocialGoal.Service;
using SocialGoal.Web.Core.Models;
using SocialGoal.Web.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNetCore.Mvc;

using Microsoft.AspNetCore.Identity;

using Microsoft.AspNetCore.Authorization;


namespace SocialGoal.Web.Controllers
{
    [Authorize]
    public class EmailRequestController : Controller
    {
        //
        // GET: /EmailRequest/
        private readonly ISecurityTokenService securityTokenService;
        public readonly IGroupUserService groupUserService;
        public readonly ISupportService supportService;
        public readonly IGroupInvitationService groupInvitationService;
        private readonly UserManager<ApplicationUser> _userManager;

        public EmailRequestController(ISecurityTokenService securityTokenService, IGroupUserService groupUserService, ISupportService supportService, IGroupInvitationService groupInvitationService, UserManager<ApplicationUser> userManager)
        {
            this.securityTokenService = securityTokenService;
            this.groupUserService = groupUserService;
            this.supportService = supportService;
            this.groupInvitationService = groupInvitationService;
            this._userManager = userManager;
        }

        public ActionResult AddGroupUser()
        {
            Guid groupIdToken = (Guid)TempData["grToken"];
            var groupId = securityTokenService.GetActualId(groupIdToken);
            GroupUser newGroupUser = new GroupUser()
            {
                UserId = _userManager.GetUserId(User),
                GroupId = groupId,
                Admin = false
            };
            groupUserService.CreateGroupUser(newGroupUser, groupInvitationService);
            securityTokenService.DeleteSecurityToken(groupIdToken);
            SocialGoalSessionFacade.Remove(SocialGoalSessionFacade.JoinGroupOrGoal);
            return RedirectToAction("Index", "Home");
        }

        public ActionResult AddSupportToGoal()
        {
            Guid goalIdToken = (Guid)TempData["goToken"];
            var goalId = securityTokenService.GetActualId(goalIdToken);
            supportService.CreateSupport(new Support() { UserId = _userManager.GetUserId(User), GoalId = goalId, SupportedDate = DateTime.Now });
            securityTokenService.DeleteSecurityToken(goalIdToken);
            SocialGoalSessionFacade.Remove(SocialGoalSessionFacade.JoinGroupOrGoal);
            return RedirectToAction("Index", "Home");
        }
    }
}
