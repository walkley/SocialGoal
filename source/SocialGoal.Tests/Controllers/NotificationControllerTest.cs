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
using SocialGoal.Web.Core.Authentication;
using SocialGoal.Web.Core.Models;
using SocialGoal.Tests.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using NUnit.Framework.Legacy;
using Microsoft.AspNetCore.Identity;
using AutoMapper;



namespace SocialGoal.Tests.Controllers
{
    [TestFixture]
    public class NotificationControllerTest
    {

        Mock<IGroupInvitationRepository> groupInvitationRepository;
        Mock<ISupportInvitationRepository> supportInvitationRepository;
        Mock<IFollowRequestRepository> followRequestRepository;
        Mock<IUserRepository> userRepository;
        Mock<IUserProfileRepository> userProfileRepository;

        Mock<IUnitOfWork> unitOfWork;

        IGoalService goalService;
        IUpdateService updateService;
        ICommentService commentService;
        IGroupInvitationService groupInvitationService;
        ISupportInvitationService supportInvitationService;
        IFollowRequestService followRequestService;
        IUserService userService;
        Mock<UserManager<ApplicationUser>> userManager;
        IMapper mapper;

        Mock<ControllerContext> controllerContext;
        Mock<IIdentity> identity;
        Mock<ClaimsPrincipal> principal;
        Mock<HttpContext> contextBase;
        Mock<HttpRequest> httpRequest;
        Mock<HttpResponse> httpResponse;
        Mock<GenericPrincipal> genericPrincipal;

        [SetUp]
        public void SetUp()
        {
            groupInvitationRepository = new Mock<IGroupInvitationRepository>();
            supportInvitationRepository = new Mock<ISupportInvitationRepository>();
            followRequestRepository = new Mock<IFollowRequestRepository>();
            userRepository = new Mock<IUserRepository>();
            userProfileRepository = new Mock<IUserProfileRepository>();

            // Setup UserManager mock
            var store = new Mock<IUserStore<ApplicationUser>>();
            userManager = new Mock<UserManager<ApplicationUser>>(store.Object, null, null, null, null, null, null, null, null);

            // Setup AutoMapper
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<GroupInvitation, NotificationViewModel>();
                cfg.CreateMap<SupportInvitation, NotificationViewModel>();
                cfg.CreateMap<FollowRequest, NotificationViewModel>();
            });
            mapper = config.CreateMapper();


            unitOfWork = new Mock<IUnitOfWork>();

            controllerContext = new Mock<ControllerContext>();
            contextBase = new Mock<HttpContext>();
            principal = new Mock<ClaimsPrincipal>();
            identity = new Mock<IIdentity>();
            httpRequest = new Mock<HttpRequest>();
            httpResponse = new Mock<HttpResponse>();
            genericPrincipal = new Mock<GenericPrincipal>();

            groupInvitationService = new GroupInvitationService(groupInvitationRepository.Object, unitOfWork.Object);
            supportInvitationService = new SupportInvitationService(supportInvitationRepository.Object, unitOfWork.Object);
            followRequestService = new FollowRequestService(followRequestRepository.Object, unitOfWork.Object);
            userService = new UserService(userRepository.Object, unitOfWork.Object, userProfileRepository.Object);
        }

        [TearDown]
        public void TearDown()
        {
        }
        [Test]
        public void Index_If_AjaxRequest_IsNull()
        {
            MemoryUser user = new MemoryUser("adarsh");
            ApplicationUser applicationUser = getApplicationUser();
            var userContext = new UserInfo
            {
                UserId = user.Id,
                DisplayName = user.UserName,
                UserIdentifier = applicationUser.Email,
                RoleName = Enum.GetName(typeof(UserRoles), applicationUser.RoleId)
            };
            // Replace FormsAuthentication with equivalent .NET Core authentication
            TimeSpan authTimeout = TimeSpan.FromMinutes(30);
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Id),
                new Claim(ClaimTypes.Email, applicationUser.Email),
                new Claim("UserContext", userContext.ToString())
            };
            var userIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var userPrincipal = new ClaimsPrincipal(userIdentity);

            userRepository.Setup(x => x.Get(It.IsAny<Expression<Func<ApplicationUser, bool>>>())).Returns(applicationUser);

            NotificationController controller = new NotificationController(goalService, updateService, commentService, groupInvitationService, supportInvitationService, followRequestService, userService, userManager.Object, mapper);


            principal.SetupGet(x => x.Identity.Name).Returns("adarsh");
            principal.SetupGet(x => x.Identity.IsAuthenticated).Returns(true);
            controllerContext.SetupGet(x => x.HttpContext.User).Returns(principal.Object);
            controller.ControllerContext = controllerContext.Object;

            contextBase.SetupGet(x => x.Request).Returns(httpRequest.Object);
            contextBase.SetupGet(x => x.Response).Returns(httpResponse.Object);
            genericPrincipal.Setup(x => x.Identity).Returns(identity.Object);

            var responseCookiesMock = new Mock<IResponseCookies>();
            httpResponse.SetupGet(a => a.Cookies).Returns(responseCookiesMock.Object);

            var formsAuthentication = new DefaultFormsAuthentication();



            // In ASP.NET Core, we manage cookies differently
            var cookieOptions = new CookieOptions
            {
                Expires = DateTime.Now.AddMinutes(30),
                Path = "/"
            };

            // Simulate authentication cookie
            contextBase.Object.Response.Cookies.Append(
                CookieAuthenticationDefaults.AuthenticationScheme,
                "authentication-cookie-value",
                cookieOptions);

            // In ASP.NET Core tests, we work with the claims principal directly
            // Create a mock of SocialGoalUser instead of instantiating it directly
            var goalsetterUser = new Mock<SocialGoalUser>().Object;
            // Setup properties through reflection since they have private setters
            typeof(SocialGoalUser).GetProperty("RoleName").SetValue(goalsetterUser, userContext.RoleName);
            typeof(SocialGoalUser).GetProperty("UserId").SetValue(goalsetterUser, userContext.UserId);
            string[] userRoles = { goalsetterUser.RoleName };

            principal.Setup(x => x.Identity).Returns(goalsetterUser);
            IEnumerable<GroupInvitation> groupInvitation = new List<GroupInvitation> {
            new GroupInvitation{ Accepted=false,ToUserId = "402bd590-fdc7-49ad-9728-40efbfe512ed", FromUserId = "402bd590-fdc7-49ad-9728-40efbfe512ec"},
            new GroupInvitation{ Accepted=false,ToUserId = "402bd590-fdc7-49ad-9728-40efbfe512ec",FromUserId = "402bd590-fdc7-49ad-9728-40efbfe512ed",},
            new GroupInvitation{ Accepted=false,ToUserId = "402bd590-fdc7-49ad-9728-40efbfe512ec",FromUserId = "402bd590-fdc7-49ad-9728-40efbfe512ee"},


          }.AsEnumerable();

            groupInvitationRepository.Setup(x => x.GetAll()).Returns(groupInvitation);

            IEnumerable<SupportInvitation> supportInvitation = new List<SupportInvitation> {
            new SupportInvitation{ Accepted=false,ToUserId = "402bd590-fdc7-49ad-9728-40efbfe512ed",FromUserId = "402bd590-fdc7-49ad-9728-40efbfe512ec"},
            new SupportInvitation{ Accepted=false,ToUserId = "402bd590-fdc7-49ad-9728-40efbfe512ec",FromUserId ="402bd590-fdc7-49ad-9728-40efbfe512ed"},
            new SupportInvitation{ Accepted=false,ToUserId = "402bd590-fdc7-49ad-9728-40efbfe512ec",FromUserId ="402bd590-fdc7-49ad-9728-40efbfe512ee"},


          }.AsEnumerable();

            supportInvitationRepository.Setup(x => x.GetAll()).Returns(supportInvitation);

            IEnumerable<FollowRequest> followInvitation = new List<FollowRequest> {
            new FollowRequest{ Accepted=false,ToUserId = "402bd590-fdc7-49ad-9728-40efbfe512ed",FromUserId = "402bd590-fdc7-49ad-9728-40efbfe512ec"},
            new FollowRequest{ Accepted=false,ToUserId = "402bd590-fdc7-49ad-9728-40efbfe512ec",FromUserId ="402bd590-fdc7-49ad-9728-40efbfe512ed"},
            new FollowRequest{ Accepted=false,ToUserId = "402bd590-fdc7-49ad-9728-40efbfe512ec",FromUserId = "402bd590-fdc7-49ad-9728-40efbfe512ee"},


          }.AsEnumerable();

            followRequestRepository.Setup(x => x.GetMany(It.IsAny<Expression<Func<FollowRequest, bool>>>())).Returns(followInvitation);

            // MapperConfiguration already set up in SetUp method

            ViewResult result = controller.Index(1) as ViewResult;
            ClassicAssert.IsNotNull(result);
        }

        [Test]
        public void Index_If_AjaxRequest_Is_NotNull()
        {
            MemoryUser user = new MemoryUser("adarsh");
            ApplicationUser applicationUser = getApplicationUser();
            var userContext = new UserInfo
            {
                UserId = user.Id,
                DisplayName = user.UserName,
                UserIdentifier = applicationUser.Email,
                RoleName = Enum.GetName(typeof(UserRoles), applicationUser.RoleId)
            };
            // Replace FormsAuthentication with equivalent .NET Core authentication
            TimeSpan authTimeout = TimeSpan.FromMinutes(30);
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Id),
                new Claim(ClaimTypes.Email, applicationUser.Email),
                new Claim("UserContext", userContext.ToString())
            };
            var userIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var userPrincipal = new ClaimsPrincipal(userIdentity);

            userRepository.Setup(x => x.Get(It.IsAny<Expression<Func<ApplicationUser, bool>>>())).Returns(applicationUser);

            NotificationController controller = new NotificationController(goalService, updateService, commentService, groupInvitationService, supportInvitationService, followRequestService, userService, userManager.Object, mapper);


            principal.SetupGet(x => x.Identity.Name).Returns("adarsh");
            principal.SetupGet(x => x.Identity.IsAuthenticated).Returns(true);
            controllerContext.SetupGet(x => x.HttpContext.User).Returns(principal.Object);
            controllerContext.SetupGet(s => s.HttpContext.Request.Headers["X-Requested-With"]).Returns("XMLHttpRequest");
            controller.ControllerContext = controllerContext.Object;


            contextBase.SetupGet(x => x.Request).Returns(httpRequest.Object);
            contextBase.SetupGet(x => x.Response).Returns(httpResponse.Object);
            genericPrincipal.Setup(x => x.Identity).Returns(identity.Object);

            var responseCookiesMock = new Mock<IResponseCookies>();
            httpResponse.SetupGet(a => a.Cookies).Returns(responseCookiesMock.Object);

            var formsAuthentication = new DefaultFormsAuthentication();



            // In ASP.NET Core, we manage cookies differently
            var cookieOptions = new CookieOptions
            {
                Expires = DateTime.Now.AddMinutes(30),
                Path = "/"
            };

            // Simulate authentication cookie
            contextBase.Object.Response.Cookies.Append(
                CookieAuthenticationDefaults.AuthenticationScheme,
                "authentication-cookie-value",
                cookieOptions);

            // In ASP.NET Core tests, we work with the claims principal directly
            // Create a mock of SocialGoalUser instead of instantiating it directly
            var goalsetterUser = new Mock<SocialGoalUser>().Object;
            // Setup properties through reflection since they have private setters
            typeof(SocialGoalUser).GetProperty("RoleName").SetValue(goalsetterUser, userContext.RoleName);
            typeof(SocialGoalUser).GetProperty("UserId").SetValue(goalsetterUser, userContext.UserId);
            string[] userRoles = { goalsetterUser.RoleName };

            principal.Setup(x => x.Identity).Returns(goalsetterUser);
            IEnumerable<GroupInvitation> groupInvitation = new List<GroupInvitation> {
            new GroupInvitation{ Accepted=false,ToUserId = "402bd590-fdc7-49ad-9728-40efbfe512ed", FromUserId = "402bd590-fdc7-49ad-9728-40efbfe512ec"},
            new GroupInvitation{ Accepted=false,ToUserId = "402bd590-fdc7-49ad-9728-40efbfe512ec",FromUserId = "402bd590-fdc7-49ad-9728-40efbfe512ed",},
            new GroupInvitation{ Accepted=false,ToUserId = "402bd590-fdc7-49ad-9728-40efbfe512ec",FromUserId = "402bd590-fdc7-49ad-9728-40efbfe512ee"},


          }.AsEnumerable();

            groupInvitationRepository.Setup(x => x.GetAll()).Returns(groupInvitation);

            IEnumerable<SupportInvitation> supportInvitation = new List<SupportInvitation> {
            new SupportInvitation{ Accepted=false,ToUserId = "402bd590-fdc7-49ad-9728-40efbfe512ed",FromUserId = "402bd590-fdc7-49ad-9728-40efbfe512ec"},
            new SupportInvitation{ Accepted=false,ToUserId = "402bd590-fdc7-49ad-9728-40efbfe512ec",FromUserId ="402bd590-fdc7-49ad-9728-40efbfe512ed"},
            new SupportInvitation{ Accepted=false,ToUserId = "402bd590-fdc7-49ad-9728-40efbfe512ec",FromUserId = "402bd590-fdc7-49ad-9728-40efbfe512ee"},


          }.AsEnumerable();

            supportInvitationRepository.Setup(x => x.GetAll()).Returns(supportInvitation);

            IEnumerable<FollowRequest> followInvitation = new List<FollowRequest> {
            new FollowRequest{ Accepted=false,ToUserId = "402bd590-fdc7-49ad-9728-40efbfe512ed",FromUserId = "402bd590-fdc7-49ad-9728-40efbfe512ec"},
            new FollowRequest{ Accepted=false,ToUserId = "402bd590-fdc7-49ad-9728-40efbfe512ec",FromUserId ="402bd590-fdc7-49ad-9728-40efbfe512ed"},
            new FollowRequest{ Accepted=false,ToUserId = "402bd590-fdc7-49ad-9728-40efbfe512ec",FromUserId = "402bd590-fdc7-49ad-9728-40efbfe512ee"},


          }.AsEnumerable();

            followRequestRepository.Setup(x => x.GetMany(It.IsAny<Expression<Func<FollowRequest, bool>>>())).Returns(followInvitation);

            // MapperConfiguration already set up in SetUp method

            PartialViewResult result = controller.Index(1) as PartialViewResult;
            ClassicAssert.IsNotNull(result);
            ClassicAssert.AreEqual("_NotificationList", result.ViewName);
        }

        [Test]
        public void Get_All_Notifications()
        {
            MemoryUser user = new MemoryUser("adarsh");
            ApplicationUser applicationUser = getApplicationUser();
            var userContext = new UserInfo
            {
                UserId = user.Id,
                DisplayName = user.UserName,
                UserIdentifier = applicationUser.Email,
                RoleName = Enum.GetName(typeof(UserRoles), applicationUser.RoleId)
            };
            // Replace FormsAuthentication with equivalent .NET Core authentication
            TimeSpan authTimeout = TimeSpan.FromMinutes(30);
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Id),
                new Claim(ClaimTypes.Email, applicationUser.Email),
                new Claim("UserContext", userContext.ToString())
            };
            var userIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var userPrincipal = new ClaimsPrincipal(userIdentity);

            userRepository.Setup(x => x.Get(It.IsAny<Expression<Func<ApplicationUser, bool>>>())).Returns(applicationUser);

            NotificationController controller = new NotificationController(goalService, updateService, commentService, groupInvitationService, supportInvitationService, followRequestService, userService, userManager.Object, mapper);


            principal.SetupGet(x => x.Identity.Name).Returns("adarsh");
            principal.SetupGet(x => x.Identity.IsAuthenticated).Returns(true);
            controllerContext.SetupGet(x => x.HttpContext.User).Returns(principal.Object);
            controller.ControllerContext = controllerContext.Object;

            contextBase.SetupGet(x => x.Request).Returns(httpRequest.Object);
            contextBase.SetupGet(x => x.Response).Returns(httpResponse.Object);
            genericPrincipal.Setup(x => x.Identity).Returns(identity.Object);

            var responseCookiesMock = new Mock<IResponseCookies>();
            httpResponse.SetupGet(a => a.Cookies).Returns(responseCookiesMock.Object);

            var formsAuthentication = new DefaultFormsAuthentication();



            // In ASP.NET Core, we manage cookies differently
            var cookieOptions = new CookieOptions
            {
                Expires = DateTime.Now.AddMinutes(30),
                Path = "/"
            };

            // Simulate authentication cookie
            contextBase.Object.Response.Cookies.Append(
                CookieAuthenticationDefaults.AuthenticationScheme,
                "authentication-cookie-value",
                cookieOptions);

            // In ASP.NET Core tests, we work with the claims principal directly
            // Create a mock of SocialGoalUser instead of instantiating it directly
            var goalsetterUser = new Mock<SocialGoalUser>().Object;
            // Setup properties through reflection since they have private setters
            typeof(SocialGoalUser).GetProperty("RoleName").SetValue(goalsetterUser, userContext.RoleName);
            typeof(SocialGoalUser).GetProperty("UserId").SetValue(goalsetterUser, userContext.UserId);
            string[] userRoles = { goalsetterUser.RoleName };

            principal.Setup(x => x.Identity).Returns(goalsetterUser);
            IEnumerable<GroupInvitation> groupInvitation = new List<GroupInvitation> {
            new GroupInvitation{ Accepted=false,ToUserId = "402bd590-fdc7-49ad-9728-40efbfe512ed", FromUserId ="402bd590-fdc7-49ad-9728-40efbfe512ec"},
            new GroupInvitation{ Accepted=false,ToUserId = "402bd590-fdc7-49ad-9728-40efbfe512ec",FromUserId = "402bd590-fdc7-49ad-9728-40efbfe512ed",},
            new GroupInvitation{ Accepted=false,ToUserId = "402bd590-fdc7-49ad-9728-40efbfe512ec",FromUserId = "402bd590-fdc7-49ad-9728-40efbfe512ee"},


          }.AsEnumerable();

            groupInvitationRepository.Setup(x => x.GetAll()).Returns(groupInvitation);

            IEnumerable<SupportInvitation> supportInvitation = new List<SupportInvitation> {
            new SupportInvitation{ Accepted=false,ToUserId = "402bd590-fdc7-49ad-9728-40efbfe512ed",FromUserId = "402bd590-fdc7-49ad-9728-40efbfe512ec"},
            new SupportInvitation{ Accepted=false,ToUserId = "402bd590-fdc7-49ad-9728-40efbfe512ec",FromUserId ="402bd590-fdc7-49ad-9728-40efbfe512ed"},
            new SupportInvitation{ Accepted=false,ToUserId = "402bd590-fdc7-49ad-9728-40efbfe512ec",FromUserId = "402bd590-fdc7-49ad-9728-40efbfe512ee"},


          }.AsEnumerable();

            supportInvitationRepository.Setup(x => x.GetAll()).Returns(supportInvitation);

            IEnumerable<FollowRequest> followInvitation = new List<FollowRequest> {
            new FollowRequest{ Accepted=false,ToUserId = "402bd590-fdc7-49ad-9728-40efbfe512ed",FromUserId = "402bd590-fdc7-49ad-9728-40efbfe512ec"},
            new FollowRequest{ Accepted=false,ToUserId = "402bd590-fdc7-49ad-9728-40efbfe512ec",FromUserId ="402bd590-fdc7-49ad-9728-40efbfe512ed"},
            new FollowRequest{ Accepted=false,ToUserId = "402bd590-fdc7-49ad-9728-40efbfe512ec",FromUserId = "402bd590-fdc7-49ad-9728-40efbfe512ee"},


          }.AsEnumerable();

            followRequestRepository.Setup(x => x.GetMany(It.IsAny<Expression<Func<FollowRequest, bool>>>())).Returns(followInvitation);

            // MapperConfiguration already set up in SetUp method

            IEnumerable<NotificationViewModel> result = controller.GetNotifications(5, 5) as IEnumerable<NotificationViewModel>;
            ClassicAssert.IsNotNull(result);
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
