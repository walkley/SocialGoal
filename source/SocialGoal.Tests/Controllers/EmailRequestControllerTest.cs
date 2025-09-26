using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SocialGoal.Data.Repository;
using SocialGoal.Data.Infrastructure;
using SocialGoal.Model.Models;
using SocialGoal.Service;
using SocialGoal.Web.Core;
using SocialGoal.Web.ViewModels;
using SocialGoal.Web.Core.Extensions;
using SocialGoal.Web.Controllers;
using NUnit.Framework;
using AutoMapper;
using Moq;
using System.Linq.Expressions;
using System.Security.Principal;
using System.Security.Claims;
using SocialGoal.Web.Core.Authentication;
// Removed System.Web.Security since we're using ASP.NET Core now
using SocialGoal.Web.Core.Models;
using System.IO;
using System.Web.SessionState;
using Microsoft.AspNetCore.Http;
using System.Reflection;
using SocialGoal.Tests.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using NUnit.Framework.Legacy;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;



namespace SocialGoal.Tests.Controllers
{
    [TestFixture]
    public class EmailRequestControllerTest
    {
        Mock<ISecurityTokenRepository> securityTokenRepository;
        Mock<IGroupUserRepository> groupUserRepository;
        Mock<ISupportRepository> supportRepository;
        Mock<IGroupInvitationRepository> groupInvitationRepository;
        Mock<IUserRepository> userRepository;
        Mock<IFollowUserRepository> followUserRepository;
        Mock<UserProfileRepository> userProfileRepository;
        Mock<UserManager<ApplicationUser>> userManager;


        Mock<IUnitOfWork> unitOfWork;

        Mock<ITempDataDictionary> tempData;
        Mock<ControllerContext> controllerContext;
        Mock<IIdentity> identity;
        Mock<ClaimsPrincipal> principal;
        Mock<HttpContext> contextBase;
        Mock<HttpRequest> httpRequest;
        Mock<HttpResponse> httpResponse;
        Mock<ClaimsPrincipal> genericPrincipal;

        public ISecurityTokenService securityTokenService;
        public IGroupUserService groupUserService;
        public ISupportService supportService;
        public IGroupInvitationService groupInvitationService;
        public IUserService userService;


        [SetUp]
        public void SetUp()
        {
            securityTokenRepository = new Mock<ISecurityTokenRepository>();
            supportRepository = new Mock<ISupportRepository>();
            groupInvitationRepository = new Mock<IGroupInvitationRepository>();
            groupUserRepository = new Mock<IGroupUserRepository>();
            userRepository = new Mock<IUserRepository>();
            followUserRepository = new Mock<IFollowUserRepository>();
            //userProfileRepository=new Mock<UserProfileRepository>();

            unitOfWork = new Mock<IUnitOfWork>();
            userManager = new Mock<UserManager<ApplicationUser>>(
                Mock.Of<IUserStore<ApplicationUser>>(),
                null, null, null, null, null, null, null, null);

            tempData = new Mock<ITempDataDictionary>();
            controllerContext = new Mock<ControllerContext>();
            contextBase = new Mock<HttpContext>();
            principal = new Mock<ClaimsPrincipal>();
            identity = new Mock<IIdentity>();
            httpRequest = new Mock<HttpRequest>();
            httpResponse = new Mock<HttpResponse>();
            genericPrincipal = new Mock<ClaimsPrincipal>();

            securityTokenService = new SecurityTokenService(securityTokenRepository.Object, unitOfWork.Object);
            groupInvitationService = new GroupInvitationService(groupInvitationRepository.Object, unitOfWork.Object);
            groupUserService = new GroupUserService(groupUserRepository.Object, userRepository.Object, unitOfWork.Object);
            supportService = new SupportService(supportRepository.Object, followUserRepository.Object, unitOfWork.Object);
            // userService = new UserService(userRepository.Object, unitOfWork.Object, userProfileRepository.Object);
        }
        [TearDown]
        public void TearDown()
        {

        }

        [Test]
        public void Add_GroupUser()
        {
            Guid guidToken = Guid.NewGuid();
            SecurityToken token = new SecurityToken()
            {
                SecurityTokenId = 1,
                Token = guidToken,
                ActualID = 1
            };

            securityTokenRepository.Setup(x => x.Get(It.IsAny<Expression<Func<SecurityToken, bool>>>())).Returns(token);

            MemoryUser user = new MemoryUser("adarsh");
            ApplicationUser applicationUser = getApplicationUser();
            var userContext = new UserInfo
            {
                UserId = user.Id,
                DisplayName = user.UserName,
                UserIdentifier = applicationUser.Email,
                RoleName = Enum.GetName(typeof(UserRoles), applicationUser.RoleId)
            };
            // In .NET Core, we use a different authentication system
            // For testing purposes, we'll create a simple object with the required properties
            var testTicket = new {
                Version = 1,
                Name = user.Id,
                IssueDate = DateTime.Now,
                Expiration = DateTime.Now.AddMinutes(30),
                IsPersistent = false,
                UserData = userContext.ToString()
            };



            EmailRequestController controller = new EmailRequestController(securityTokenService, groupUserService, supportService, groupInvitationService, userManager.Object);

            controllerContext.SetupGet(x => x.HttpContext.User).Returns(principal.Object);
            principal.SetupGet(p => p.Identity.IsAuthenticated).Returns(true);
            controller.ControllerContext = controllerContext.Object;

            contextBase.SetupGet(x => x.Request).Returns(httpRequest.Object);
            contextBase.SetupGet(x => x.Response).Returns(httpResponse.Object);
            genericPrincipal.Setup(x => x.Identity).Returns(identity.Object);

            var mockResponseCookies = new Mock<IResponseCookies>();
            httpResponse.SetupGet(a => a.Cookies).Returns(mockResponseCookies.Object);

            // Skip cookie operations in test environment
            // SocialGoalUser may not accept a single parameter in .NET 8
            // Create the identity with the necessary information directly
            var goalsetterUser = new SocialGoalUser(); // Use parameterless constructor
            // Set any required properties after construction
            principal.Setup(x => x.Identity).Returns(goalsetterUser);

            // Modern ASP.NET Core approach - no need to instantiate HttpRequest directly
            // The context has already been set up through the Mock objects above

            controller.TempData = tempData.Object;
            controller.TempData["grToken"] = guidToken;
            var result = controller.AddGroupUser() as RedirectToRouteResult;
            ClassicAssert.AreEqual("Index", result.RouteValues["action"]);
        }

        [Test]
        public void Add_Support_ToGoal()
        {

            Guid guidToken = Guid.NewGuid();
            SecurityToken token = new SecurityToken()
            {
                SecurityTokenId = 1,
                Token = guidToken,
                ActualID = 1
            };

            securityTokenRepository.Setup(x => x.Get(It.IsAny<Expression<Func<SecurityToken, bool>>>())).Returns(token);

            MemoryUser user = new MemoryUser("adarsh");
            ApplicationUser applicationUser = getApplicationUser();
            var userContext = new UserInfo
            {
                UserId = user.Id,
                DisplayName = user.UserName,
                UserIdentifier = applicationUser.Email,
                RoleName = Enum.GetName(typeof(UserRoles), applicationUser.RoleId)
            };
            // In .NET Core, we use a different authentication system
            // For testing purposes, we'll create a simple object with the required properties
            var testTicket = new {
                Version = 1,
                Name = user.Id,
                IssueDate = DateTime.Now,
                Expiration = DateTime.Now.AddMinutes(30),
                IsPersistent = false,
                UserData = userContext.ToString()
            };



            EmailRequestController controller = new EmailRequestController(securityTokenService, groupUserService, supportService, groupInvitationService, userManager.Object);

            controllerContext.SetupGet(x => x.HttpContext.User).Returns(principal.Object);
            principal.SetupGet(p => p.Identity.IsAuthenticated).Returns(true);
            controller.ControllerContext = controllerContext.Object;

            contextBase.SetupGet(x => x.Request).Returns(httpRequest.Object);
            contextBase.SetupGet(x => x.Response).Returns(httpResponse.Object);
            genericPrincipal.Setup(x => x.Identity).Returns(identity.Object);

            var mockResponseCookies = new Mock<IResponseCookies>();
            httpResponse.SetupGet(a => a.Cookies).Returns(mockResponseCookies.Object);

            // Skip cookie operations in test environment
            // SocialGoalUser may not accept a single parameter in .NET 8
            // Create the identity with the necessary information directly
            var goalsetterUser = new SocialGoalUser(); // Use parameterless constructor
            // Set any required properties after construction
            principal.Setup(x => x.Identity).Returns(goalsetterUser);

            // Modern ASP.NET Core approach - no need to instantiate HttpRequest directly
            // The context has already been set up through the Mock objects above

            controller.TempData = tempData.Object;
            controller.TempData["goToken"] = guidToken;
            var result = controller.AddSupportToGoal() as RedirectToRouteResult;
            ClassicAssert.AreEqual("Index", result.RouteValues["action"]);
        }

        public ApplicationUser getApplicationUser()
        {
            ApplicationUser applicationUser = new ApplicationUser()
            {
                Activated = true,
                Email = "adarsh@foo.com",
                FirstName = "Adarsh",
                LastName = "Vikraman",
                UserName = "adarsh",
                RoleId = 0,
                Id = "402bd590-fdc7-49ad-9728-40efbfe512ec",
                DateCreated = DateTime.Now,
                LastLoginTime = DateTime.Now,
                ProfilePicUrl = null,
            };
            return applicationUser;
        }
    }
}
