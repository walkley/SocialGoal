using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SocialGoal.Web.Controllers;
using NUnit.Framework;
using SocialGoal.Model.Models;
using SocialGoal.Tests.Helpers;
using SocialGoal.Data.Repository;
using Moq;
using SocialGoal.Data.Infrastructure;
using System.Security.Principal;
using System.Security.Claims;
using System.IO;
using SocialGoal.Web.Core.Authentication;
using SocialGoal.Service;
using SocialGoal.Web.Mailers;
using Mvc.Mailer;
using SocialGoal.Models;
using System.Linq.Expressions;
// System.Web.Security is not available in .NET Core
using SocialGoal.Web.Core.Models;
using SocialGoal.Web.ViewModels;
using AutoMapper;
// using System.Web.SessionState; // Not available in .NET Core
using System.Reflection;
using System.Collections.Specialized;
using NUnit.Framework.Legacy;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;

namespace SocialGoal.Web.Controllers.Test
{
    [TestFixture()]
    public class AccountControllerTest
    {
        Mock<IUserRepository> userRepository;
        Mock<IUserProfileRepository> userProfileRepository;
        Mock<IFollowRequestRepository> followRequestRepository;
        Mock<IFollowUserRepository> followUserRepository;
        Mock<ISecurityTokenRepository> securityTokenRepository;
        Mock<IUnitOfWork> unitOfWork;
        Mock<ControllerContext> controllerContext;
        Mock<IIdentity> identity;
        Mock<IPrincipal> principal;
        Mock<HttpContext> httpContext;
        Mock<HttpContext> contextBase;
        Mock<HttpRequest> httpRequest;
        Mock<HttpResponse> httpResponse;
        Mock<ISession> httpSession;
        Mock<GenericPrincipal> genericPrincipal;
        Mock<IResponseCookies> responseCookies;

        Mock<ITempDataDictionary> tempData;
        Mock<IFormFile> file;
        Mock<Stream> stream;
        Mock<IFormsAuthentication> authentication;
        Mock<SignInManager<ApplicationUser>> mockSignInManager;
        Mock<IMapper> mockMapper;
        Mock<IWebHostEnvironment> mockWebHostEnvironment;

        IUserService userService;
        IUserProfileService userProfileService;
        IGoalService goalService;
        IUpdateService updateService;
        ICommentService commentService;
        IFollowRequestService followRequestService;
        IFollowUserService followUserService;
        ISecurityTokenService securityTokenService;
        IUserMailer userMailer = new UserMailer();
        Mock<AccountController> accountController;


        [SetUp]
        public void SetUp()
        {
            userRepository = new Mock<IUserRepository>();
            userProfileRepository = new Mock<IUserProfileRepository>();
            followRequestRepository = new Mock<IFollowRequestRepository>();
            followUserRepository = new Mock<IFollowUserRepository>();
            securityTokenRepository = new Mock<ISecurityTokenRepository>();


            unitOfWork = new Mock<IUnitOfWork>();

            userService = new UserService(userRepository.Object, unitOfWork.Object, userProfileRepository.Object);
            userProfileService = new UserProfileService(userProfileRepository.Object, unitOfWork.Object);
            followRequestService = new FollowRequestService(followRequestRepository.Object, unitOfWork.Object);
            followUserService = new FollowUserService(followUserRepository.Object, unitOfWork.Object);
            securityTokenService = new SecurityTokenService(securityTokenRepository.Object, unitOfWork.Object);

            controllerContext = new Mock<ControllerContext>();
            contextBase = new Mock<HttpContext>();
            httpRequest = new Mock<HttpRequest>();
            httpResponse = new Mock<HttpResponse>();
            genericPrincipal = new Mock<GenericPrincipal>();
            httpSession = new Mock<ISession>();
            authentication = new Mock<IFormsAuthentication>();
            responseCookies = new Mock<IResponseCookies>();


            identity = new Mock<IIdentity>();
            principal = new Mock<IPrincipal>();
            tempData = new Mock<ITempDataDictionary>();
            file = new Mock<IFormFile>();
            stream = new Mock<Stream>();
            accountController = new Mock<AccountController>();
            mockMapper = new Mock<IMapper>();
            mockWebHostEnvironment = new Mock<IWebHostEnvironment>();
        }

        [TearDown]
        public void TearDown()
        {
            TestSmtpClient.SentMails.Clear();
        }

        [Test]
        public void SearchUser()
        {
            var userManager = CreateUserManager();
            IEnumerable<ApplicationUser> fake = new List<ApplicationUser>
            {
             new ApplicationUser{Activated=true,Email="user1@foo.com",FirstName="user1",LastName="user1",RoleId=0},
              new ApplicationUser{Activated=true,Email="user2@foo.com",FirstName="user2",LastName="user2",RoleId=0},
              new ApplicationUser{Activated=true,Email="user3@foo.com",FirstName="user3",LastName="user3",RoleId=0},
              new ApplicationUser{Activated=true,Email="user4@foo.com",FirstName="user4",LastName="user4",RoleId=0}
          }.AsEnumerable();
            userRepository.Setup(x => x.GetMany(It.IsAny<Expression<Func<ApplicationUser, bool>>>())).Returns(fake);
            AccountController contr = new AccountController(userService, userProfileService, goalService, updateService, commentService, followRequestService, followUserService, securityTokenService, userManager, mockMapper.Object, mockWebHostEnvironment.Object);
            IEnumerable<ApplicationUser> result = contr.SearchUser("u") as IEnumerable<ApplicationUser>;
            ClassicAssert.IsNotNull(result);
            ClassicAssert.AreEqual(4, result.Count(), "not matching");
        }

        [Test]
        public void Image_Upload_GetView()
        {
            var userManager = CreateUserManager();
            MemoryUser user = new MemoryUser("adarsh");
            ApplicationUser applicationUser = getApplicationUser();
            var userContext = new UserInfo
            {
                UserId = user.Id,
                DisplayName = user.UserName,
                UserIdentifier = applicationUser.Email,
                RoleName = Enum.GetName(typeof(UserRoles), applicationUser.RoleId)
            };
            // Replace FormsAuthenticationTicket with Claims-based approach
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Id),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Email, applicationUser.Email),
                new Claim(ClaimTypes.Role, Enum.GetName(typeof(UserRoles), applicationUser.RoleId))
            };
            var claimsIdentity = new ClaimsIdentity(claims, "TestAuthType");
            var testPrincipal = new ClaimsPrincipal(claimsIdentity);
            // Store user context info in claims
            claims.Add(new Claim("UserContext", userContext.ToString()));
            AccountController controller = new AccountController(userService, userProfileService, goalService, updateService, commentService, followRequestService, followUserService, securityTokenService, userManager, mockMapper.Object, mockWebHostEnvironment.Object);
            principal.SetupGet(x => x.Identity.Name).Returns("adarsh");
            var claimsPrincipal = new Mock<System.Security.Claims.ClaimsPrincipal>();
            claimsPrincipal.Setup(x => x.Identity).Returns(identity.Object);
            controllerContext.SetupGet(x => x.HttpContext.User).Returns(claimsPrincipal.Object);
            controllerContext.SetupGet(p => p.HttpContext.User.Identity.IsAuthenticated).Returns(true);
            controller.ControllerContext = controllerContext.Object;
            contextBase.SetupGet(x => x.Request).Returns(httpRequest.Object);
            contextBase.SetupGet(x => x.Response).Returns(httpResponse.Object);
            genericPrincipal.Setup(x => x.Identity).Returns(identity.Object);
            contextBase.SetupGet(a => a.Response.Cookies).Returns(responseCookies.Object);
            var formsAuthentication = new DefaultFormsAuthentication();
// Mock the authentication in context without using FormsAuthentication
            contextBase.SetupGet(x => x.User).Returns(testPrincipal);
            // Mock the cookie access for testing
            string authCookieValue = "mock-cookie-value"; // Simulated encrypted cookie value
            var mockCookies = new Mock<IRequestCookieCollection>();
            mockCookies.Setup(c => c[".AspNetCore.Cookies"]).Returns(authCookieValue); // Use standard ASP.NET Core cookie name
            contextBase.SetupGet(a => a.Request.Cookies).Returns(mockCookies.Object);
// Create SocialGoalUser directly using the userContext we already set up
            var goalsetterUser = new SocialGoalUser(testPrincipal.Identity.Name, userContext);
            string[] userRoles = { goalsetterUser.RoleName };
            principal.Setup(x => x.Identity).Returns(goalsetterUser);
            userRepository.Setup(x => x.Get(It.IsAny<Expression<Func<ApplicationUser, bool>>>())).Returns(applicationUser);
            PartialViewResult result = controller.ImageUpload() as PartialViewResult;
            ClassicAssert.IsNotNull(result);
            ClassicAssert.IsInstanceOf(typeof(UploadImageViewModel), result.ViewData.Model, "Wrong model");
            var data = result.ViewData.Model as UploadImageViewModel;
            ClassicAssert.AreEqual(null, data.LocalPath, "not matching");
        }

        [Test]
        public void Upload_Image_Post()
        {
            var userManager = CreateUserManager();
            UploadImageViewModel image = new UploadImageViewModel()
            {
                IsFile = true,
                UserId = "402bd590-fdc7-49ad-9728-40efbfe512ec",
                LocalPath = "dddd"
            };
            AccountController controller = new AccountController(userService, userProfileService, goalService, updateService, commentService, followRequestService, followUserService, securityTokenService, userManager, mockMapper.Object, mockWebHostEnvironment.Object);
            ViewResult result = controller.UploadImage(image) as ViewResult;
            ClassicAssert.IsNotNull(result);
            ClassicAssert.IsInstanceOf(typeof(UploadImageViewModel), result.ViewData.Model, "WrongType");
            ClassicAssert.AreEqual("ImageUpload", result.ViewName);
        }

        [Test]
        public void UserProfile()
        {
            var userManager = CreateUserManager();
            MemoryUser user = new MemoryUser("adarsh");
            ApplicationUser applicationUser = getApplicationUser();
            var userContext = new UserInfo
            {
                UserId = user.Id,
                DisplayName = user.UserName,
                UserIdentifier = applicationUser.Email,
                RoleName = Enum.GetName(typeof(UserRoles), applicationUser.RoleId)
            };
            // Replace FormsAuthenticationTicket with Claims-based approach
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Id),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Email, applicationUser.Email),
                new Claim(ClaimTypes.Role, Enum.GetName(typeof(UserRoles), applicationUser.RoleId))
            };
            var claimsIdentity = new ClaimsIdentity(claims, "TestAuthType");
            var testPrincipal = new ClaimsPrincipal(claimsIdentity);
            // Store user context info in claims
            claims.Add(new Claim("UserContext", userContext.ToString()));
            userRepository.Setup(x => x.Get(It.IsAny<Expression<Func<ApplicationUser, bool>>>())).Returns(applicationUser);
            AccountController controller = new AccountController(userService, userProfileService, goalService, updateService, commentService, followRequestService, followUserService, securityTokenService, userManager, mockMapper.Object, mockWebHostEnvironment.Object);
            principal.SetupGet(x => x.Identity.Name).Returns("adarsh");
            var claimsPrincipal = new Mock<System.Security.Claims.ClaimsPrincipal>();
            claimsPrincipal.Setup(x => x.Identity).Returns(identity.Object);
            controllerContext.SetupGet(x => x.HttpContext.User).Returns(claimsPrincipal.Object);
            controllerContext.SetupGet(p => p.HttpContext.User.Identity.IsAuthenticated).Returns(true);
            controller.ControllerContext = controllerContext.Object;
            contextBase.SetupGet(x => x.Request).Returns(httpRequest.Object);
            contextBase.SetupGet(x => x.Response).Returns(httpResponse.Object);
            genericPrincipal.Setup(x => x.Identity).Returns(identity.Object);
            contextBase.SetupGet(a => a.Response.Cookies).Returns(responseCookies.Object);
            var formsAuthentication = new DefaultFormsAuthentication();
// Mock the authentication in context without using FormsAuthentication
            contextBase.SetupGet(x => x.User).Returns(testPrincipal);
            // Mock the cookie access for testing
            string authCookieValue = "mock-cookie-value"; // Simulated encrypted cookie value
            var mockCookies = new Mock<IRequestCookieCollection>();
            mockCookies.Setup(c => c[".AspNetCore.Cookies"]).Returns(authCookieValue); // Use standard ASP.NET Core cookie name
            contextBase.SetupGet(a => a.Request.Cookies).Returns(mockCookies.Object);
// Create SocialGoalUser directly using the userContext we already set up
            var goalsetterUser = new SocialGoalUser(testPrincipal.Identity.Name, userContext);
            string[] userRoles = { goalsetterUser.RoleName };
            principal.Setup(x => x.Identity).Returns(goalsetterUser);
            UserProfile prfil = new UserProfile()
            {
                FirstName="Adarsh",
                LastName="Vikraman",
                DateOfBirth = DateTime.Now,
                Gender = true,
                Address = "a",
                City = "a",
                State = "a",
                Country = "a",
                ZipCode = 2344545,
                ContactNo = 1223344556,
                UserId = "402bd590-fdc7-49ad-9728-40efbfe512ec"
            };
            userProfileRepository.Setup(x => x.Get(It.IsAny<Expression<Func<UserProfile, bool>>>())).Returns(prfil);
            IEnumerable<FollowRequest> fake = new List<FollowRequest> {
            new FollowRequest { FollowRequestId =1, FromUserId = "402bd590-fdc7-49ad-9728-40efbfe512ec", ToUserId = "402bd590-fdc7-49ad-9728-40efbfe512ed"},
            new FollowRequest { FollowRequestId =2, FromUserId = "402bd590-fdc7-49ad-9728-40efbfe512ec", ToUserId = "402bd590-fdc7-49ad-9728-40efbfe512ee"},
            };
            followRequestRepository.Setup(x => x.GetMany(It.IsAny<Expression<Func<FollowRequest, bool>>>())).Returns(fake);
            IEnumerable<FollowUser> fakeuser = new List<FollowUser> {
            new FollowUser {FollowUserId =1, Accepted = false,FromUserId = "402bd590-fdc7-49ad-9728-40efbfe512ec", ToUserId = "402bd590-fdc7-49ad-9728-40efbfe512ed"},
            new FollowUser {FollowUserId =2, Accepted = false,FromUserId = "402bd590-fdc7-49ad-9728-40efbfe512ec", ToUserId = "402bd590-fdc7-49ad-9728-40efbfe512ee" },
            };
            followUserRepository.Setup(x => x.GetMany(It.IsAny<Expression<Func<FollowUser, bool>>>())).Returns(fakeuser);
            ViewResult result = controller.UserProfile("402bd590-fdc7-49ad-9728-40efbfe512ec") as ViewResult;
            ClassicAssert.IsNotNull(result);
            ClassicAssert.IsInstanceOf(typeof(UserProfileViewModel), result.ViewData.Model, "WrongType");
            var data = result.ViewData.Model as UserProfileViewModel;
            ClassicAssert.AreEqual("adarsh", data.UserName);
        }

        [Test]
        public void Edit_Basic_Info()
        {
            var userManager = CreateUserManager();
            MemoryUser user = new MemoryUser("adarsh");
            ApplicationUser applicationUser = getApplicationUser();
            var userContext = new UserInfo
            {
                UserId = user.Id,
                DisplayName = user.UserName,
                UserIdentifier = applicationUser.Email,
                RoleName = Enum.GetName(typeof(UserRoles), applicationUser.RoleId)
            };
            // Replace FormsAuthenticationTicket with Claims-based approach
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Id),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Email, applicationUser.Email),
                new Claim(ClaimTypes.Role, Enum.GetName(typeof(UserRoles), applicationUser.RoleId))
            };
            var claimsIdentity = new ClaimsIdentity(claims, "TestAuthType");
            var testPrincipal = new ClaimsPrincipal(claimsIdentity);
            // Store user context info in claims
            claims.Add(new Claim("UserContext", userContext.ToString()));
            AccountController controller = new AccountController(userService, userProfileService, goalService, updateService, commentService, followRequestService, followUserService, securityTokenService, userManager, mockMapper.Object, mockWebHostEnvironment.Object);
            principal.SetupGet(x => x.Identity.Name).Returns("adarsh");
            var claimsPrincipal = new Mock<System.Security.Claims.ClaimsPrincipal>();
            claimsPrincipal.Setup(x => x.Identity).Returns(identity.Object);
            controllerContext.SetupGet(x => x.HttpContext.User).Returns(claimsPrincipal.Object);
            controllerContext.SetupGet(p => p.HttpContext.User.Identity.IsAuthenticated).Returns(true);
            controller.ControllerContext = controllerContext.Object;
            contextBase.SetupGet(x => x.Request).Returns(httpRequest.Object);
            contextBase.SetupGet(x => x.Response).Returns(httpResponse.Object);
            genericPrincipal.Setup(x => x.Identity).Returns(identity.Object);
            contextBase.SetupGet(a => a.Response.Cookies).Returns(responseCookies.Object);
            var formsAuthentication = new DefaultFormsAuthentication();
// Mock the authentication in context without using FormsAuthentication
            contextBase.SetupGet(x => x.User).Returns(testPrincipal);
            // Mock the cookie access for testing
            string authCookieValue = "mock-cookie-value"; // Simulated encrypted cookie value
            var mockCookies = new Mock<IRequestCookieCollection>();
            mockCookies.Setup(c => c[".AspNetCore.Cookies"]).Returns(authCookieValue); // Use standard ASP.NET Core cookie name
            contextBase.SetupGet(a => a.Request.Cookies).Returns(mockCookies.Object);
// Create SocialGoalUser directly using the userContext we already set up
            var goalsetterUser = new SocialGoalUser(testPrincipal.Identity.Name, userContext);
            string[] userRoles = { goalsetterUser.RoleName };
            principal.Setup(x => x.Identity).Returns(goalsetterUser);
            UserProfile userProfile = new UserProfile()
            {
                UserId = "402bd590-fdc7-49ad-9728-40efbfe512ec",
                FirstName = "Adarsh",
                LastName = "Vikraman",
                Email = "adarsh@foo.com",
            };
            userProfileRepository.Setup(x => x.Get(It.IsAny<Expression<Func<UserProfile, bool>>>())).Returns(userProfile);
            var config = new MapperConfiguration(cfg =>
                cfg.CreateMap<UserProfile, UserProfileFormModel>()
            );
            var mapper = config.CreateMapper();
            PartialViewResult result = controller.EditBasicInfo() as PartialViewResult;
            ClassicAssert.IsNotNull(result);
            ClassicAssert.IsInstanceOf(typeof(UserProfileFormModel), result.ViewData.Model, "WrongType");
            var data = result.ViewData.Model as UserProfileFormModel;
            ClassicAssert.AreEqual("adarsh@foo.com", data.Email);
        }

        [Test]
        public void Edit_Personal_Info()
        {
            var userManager = CreateUserManager();
            MemoryUser user = new MemoryUser("adarsh");
            ApplicationUser applicationUser = getApplicationUser();
            var userContext = new UserInfo
            {
                UserId = user.Id,
                DisplayName = user.UserName,
                UserIdentifier = applicationUser.Email,
                RoleName = Enum.GetName(typeof(UserRoles), applicationUser.RoleId)
            };
            // Replace FormsAuthenticationTicket with Claims-based approach
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Id),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Email, applicationUser.Email),
                new Claim(ClaimTypes.Role, Enum.GetName(typeof(UserRoles), applicationUser.RoleId))
            };
            var claimsIdentity = new ClaimsIdentity(claims, "TestAuthType");
            var testPrincipal = new ClaimsPrincipal(claimsIdentity);
            // Store user context info in claims
            claims.Add(new Claim("UserContext", userContext.ToString()));
            AccountController controller = new AccountController(userService, userProfileService, goalService, updateService, commentService, followRequestService, followUserService, securityTokenService, userManager, mockMapper.Object, mockWebHostEnvironment.Object);
            principal.SetupGet(x => x.Identity.Name).Returns("adarsh");
            var claimsPrincipal = new Mock<System.Security.Claims.ClaimsPrincipal>();
            claimsPrincipal.Setup(x => x.Identity).Returns(identity.Object);
            controllerContext.SetupGet(x => x.HttpContext.User).Returns(claimsPrincipal.Object);
            controllerContext.SetupGet(p => p.HttpContext.User.Identity.IsAuthenticated).Returns(true);
            controller.ControllerContext = controllerContext.Object;
            contextBase.SetupGet(x => x.Request).Returns(httpRequest.Object);
            contextBase.SetupGet(x => x.Response).Returns(httpResponse.Object);
            genericPrincipal.Setup(x => x.Identity).Returns(identity.Object);
            contextBase.SetupGet(a => a.Response.Cookies).Returns(responseCookies.Object);
            var formsAuthentication = new DefaultFormsAuthentication();
// Mock the authentication in context without using FormsAuthentication
            contextBase.SetupGet(x => x.User).Returns(testPrincipal);
            // Mock the cookie access for testing
            string authCookieValue = "mock-cookie-value"; // Simulated encrypted cookie value
            var mockCookies = new Mock<IRequestCookieCollection>();
            mockCookies.Setup(c => c[".AspNetCore.Cookies"]).Returns(authCookieValue); // Use standard ASP.NET Core cookie name
            contextBase.SetupGet(a => a.Request.Cookies).Returns(mockCookies.Object);
// Create SocialGoalUser directly using the userContext we already set up
            var goalsetterUser = new SocialGoalUser(testPrincipal.Identity.Name, userContext);
            string[] userRoles = { goalsetterUser.RoleName };
            principal.Setup(x => x.Identity).Returns(goalsetterUser);
            UserProfile grpuser = new UserProfile()
            {
                UserId = "402bd590-fdc7-49ad-9728-40efbfe512ec",
                Address = "t",
                City = "t",
                State = "adarsh@foo.com",
            };
            userProfileRepository.Setup(x => x.Get(It.IsAny<Expression<Func<UserProfile, bool>>>())).Returns(grpuser);
            var config = new MapperConfiguration(cfg =>
                cfg.CreateMap<UserProfile, UserProfileFormModel>()
            );
            var mapper = config.CreateMapper();

            PartialViewResult result = controller.EditPersonalInfo() as PartialViewResult;
            ClassicAssert.IsNotNull(result);
            ClassicAssert.IsInstanceOf(typeof(UserProfileFormModel), result.ViewData.Model, "WrongType");
            var data = result.ViewData.Model as UserProfileFormModel;
            ClassicAssert.AreEqual("t", data.Address);
        }

        [Test]
        public void Editprofile_Post()
        {
            var userManager = CreateUserManager();
            ApplicationUser applicationUser = getApplicationUser();
            userRepository.Setup(x => x.Get(It.IsAny<Expression<Func<ApplicationUser, bool>>>())).Returns(applicationUser);
            UserProfileFormModel profile = new UserProfileFormModel();

            var config = new MapperConfiguration(cfg =>
                cfg.CreateMap<UserProfileFormModel, UserProfile>()
            );
            var mapper = config.CreateMapper();
            profile.FirstName ="adarsh";

            AccountController contr = new AccountController(userService, userProfileService, goalService, updateService, commentService, followRequestService, followUserService, securityTokenService, userManager, mockMapper.Object, mockWebHostEnvironment.Object);
            var result = contr.EditProfile(profile) as RedirectToRouteResult;

            ClassicAssert.AreEqual("UserProfile", result.RouteValues["action"]);
        }

        [Test]
        public void Follow_Request()
        {
            var userManager = CreateUserManager();
            MemoryUser user = new MemoryUser("adarsh");
            ApplicationUser applicationUser = getApplicationUser();
            var userContext = new UserInfo
            {
                UserId = user.Id,
                DisplayName = user.UserName,
                UserIdentifier = applicationUser.Email,
                RoleName = Enum.GetName(typeof(UserRoles), applicationUser.RoleId)
            };
            // Replace FormsAuthenticationTicket with Claims-based approach
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Id),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Email, applicationUser.Email),
                new Claim(ClaimTypes.Role, Enum.GetName(typeof(UserRoles), applicationUser.RoleId))
            };
            var claimsIdentity = new ClaimsIdentity(claims, "TestAuthType");
            var testPrincipal = new ClaimsPrincipal(claimsIdentity);
            // Store user context info in claims
            claims.Add(new Claim("UserContext", userContext.ToString()));



            AccountController controller = new AccountController(userService, userProfileService, goalService, updateService, commentService, followRequestService, followUserService, securityTokenService, userManager, mockMapper.Object, mockWebHostEnvironment.Object);
            principal.SetupGet(x => x.Identity.Name).Returns("adarsh");
            var claimsPrincipal = new Mock<System.Security.Claims.ClaimsPrincipal>();
            claimsPrincipal.Setup(x => x.Identity).Returns(identity.Object);
            controllerContext.SetupGet(x => x.HttpContext.User).Returns(claimsPrincipal.Object);
            controllerContext.SetupGet(p => p.HttpContext.User.Identity.IsAuthenticated).Returns(true);
            controller.ControllerContext = controllerContext.Object;

            contextBase.SetupGet(x => x.Request).Returns(httpRequest.Object);
            contextBase.SetupGet(x => x.Response).Returns(httpResponse.Object);
            genericPrincipal.Setup(x => x.Identity).Returns(identity.Object);

            contextBase.SetupGet(a => a.Response.Cookies).Returns(responseCookies.Object);

            var formsAuthentication = new DefaultFormsAuthentication();



// Mock the authentication in context without using FormsAuthentication
            contextBase.SetupGet(x => x.User).Returns(testPrincipal);

            // Mock the cookie access for testing
            string authCookieValue = "mock-cookie-value"; // Simulated encrypted cookie value
            var mockCookies = new Mock<IRequestCookieCollection>();
            mockCookies.Setup(c => c[".AspNetCore.Cookies"]).Returns(authCookieValue); // Use standard ASP.NET Core cookie name
            contextBase.SetupGet(a => a.Request.Cookies).Returns(mockCookies.Object);

// Create SocialGoalUser directly using the userContext we already set up
            var goalsetterUser = new SocialGoalUser(testPrincipal.Identity.Name, userContext);
            string[] userRoles = { goalsetterUser.RoleName };

            principal.Setup(x => x.Identity).Returns(goalsetterUser);

            userRepository.Setup(x => x.Get(It.IsAny<Expression<Func<ApplicationUser, bool>>>())).Returns(applicationUser);
            var config = new MapperConfiguration(cfg =>
                cfg.CreateMap<FollowRequestFormModel, FollowRequest>()
            );
            var mapper = config.CreateMapper();

            var result = controller.FollowRequest("402bd590-fdc7-49ad-9728-40efbfe512ec") as RedirectToRouteResult;
            ClassicAssert.AreEqual("UserProfile", result.RouteValues["action"]);
        }

        [Test]
        public void Accept_Request()
        {
            var userManager = CreateUserManager();
            ApplicationUser applicationUser = getApplicationUser();
            userRepository.Setup(x => x.Get(It.IsAny<Expression<Func<ApplicationUser, bool>>>())).Returns(applicationUser);
            AccountController contr = new AccountController(userService, userProfileService, goalService, updateService, commentService, followRequestService, followUserService, securityTokenService, userManager, mockMapper.Object, mockWebHostEnvironment.Object);
            var result = contr.AcceptRequest("402bd590-fdc7-49ad-9728-40efbfe512ed","402bd590-fdc7-49ad-9728-40efbfe512ec") as RedirectToRouteResult;
            ClassicAssert.AreEqual("Index", result.RouteValues["action"]);

        }

        [Test]
        public void Delete_Follow_Request()
        {
            var userManager = CreateUserManager();
            FollowRequest request = new FollowRequest()
            {
                FromUserId = "402bd590-fdc7-49ad-9728-40efbfe512ec",
                ToUserId = "402bd590-fdc7-49ad-9728-40efbfe512ed",

            };
            followRequestRepository.Setup(x => x.Get(It.IsAny<Expression<Func<FollowRequest, bool>>>())).Returns(request);
            AccountController contr = new AccountController(userService, userProfileService, goalService, updateService, commentService, followRequestService, followUserService, securityTokenService, userManager, mockMapper.Object, mockWebHostEnvironment.Object);
            var result = contr.RejectRequest("402bd590-fdc7-49ad-9728-40efbfe512ed","402bd590-fdc7-49ad-9728-40efbfe512ec") as RedirectToRouteResult;
            ClassicAssert.AreEqual("Index", result.RouteValues["action"]);
        }
        [Test]
        public void UnFollow()
        {
            var userManager = CreateUserManager();
            MemoryUser user = new MemoryUser("adarsh");
            ApplicationUser applicationUser = getApplicationUser();
            var userContext = new UserInfo
            {
                UserId = user.Id,
                DisplayName = user.UserName,
                UserIdentifier = applicationUser.Email,
                RoleName = Enum.GetName(typeof(UserRoles), applicationUser.RoleId)
            };
            // Replace FormsAuthenticationTicket with Claims-based approach
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Id),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Email, applicationUser.Email),
                new Claim(ClaimTypes.Role, Enum.GetName(typeof(UserRoles), applicationUser.RoleId))
            };
            var claimsIdentity = new ClaimsIdentity(claims, "TestAuthType");
            var testPrincipal = new ClaimsPrincipal(claimsIdentity);
            // Store user context info in claims
            claims.Add(new Claim("UserContext", userContext.ToString()));



            AccountController controller = new AccountController(userService, userProfileService, goalService, updateService, commentService, followRequestService, followUserService, securityTokenService, userManager, mockMapper.Object, mockWebHostEnvironment.Object);

            principal.SetupGet(x => x.Identity.Name).Returns("adarsh");
            var claimsPrincipal = new Mock<System.Security.Claims.ClaimsPrincipal>();
            claimsPrincipal.Setup(x => x.Identity).Returns(identity.Object);
            controllerContext.SetupGet(x => x.HttpContext.User).Returns(claimsPrincipal.Object);
            controllerContext.SetupGet(p => p.HttpContext.User.Identity.IsAuthenticated).Returns(true);
            controller.ControllerContext = controllerContext.Object;

            contextBase.SetupGet(x => x.Request).Returns(httpRequest.Object);
            contextBase.SetupGet(x => x.Response).Returns(httpResponse.Object);
            genericPrincipal.Setup(x => x.Identity).Returns(identity.Object);

            contextBase.SetupGet(a => a.Response.Cookies).Returns(responseCookies.Object);

            var formsAuthentication = new DefaultFormsAuthentication();



// Mock the authentication in context without using FormsAuthentication
            contextBase.SetupGet(x => x.User).Returns(testPrincipal);

            // Mock the cookie access for testing
            string authCookieValue = "mock-cookie-value"; // Simulated encrypted cookie value
            var mockCookies = new Mock<IRequestCookieCollection>();
            mockCookies.Setup(c => c[".AspNetCore.Cookies"]).Returns(authCookieValue); // Use standard ASP.NET Core cookie name
            contextBase.SetupGet(a => a.Request.Cookies).Returns(mockCookies.Object);

// Create SocialGoalUser directly using the userContext we already set up
            var goalsetterUser = new SocialGoalUser(testPrincipal.Identity.Name, userContext);
            string[] userRoles = { goalsetterUser.RoleName };

            principal.Setup(x => x.Identity).Returns(goalsetterUser);
            FollowUser flwuser = new FollowUser()
            {
                FromUserId = "402bd590-fdc7-49ad-9728-40efbfe512ec",
                ToUserId = "402bd590-fdc7-49ad-9728-40efbfe512ed",

            };
            followUserRepository.Setup(x => x.Get(It.IsAny<Expression<Func<FollowUser, bool>>>())).Returns(flwuser);

            var result = controller.Unfollow("402bd590-fdc7-49ad-9728-40efbfe512ed") as RedirectToRouteResult;
            ClassicAssert.AreEqual("UserProfile", result.RouteValues["action"]);
        }

        [Test]
        public void Followers_List()
        {
            var userManager = CreateUserManager();
            MemoryUser user = new MemoryUser("adarsh");
            ApplicationUser applicationUser = getApplicationUser();
            var userContext = new UserInfo
            {
                UserId = user.Id,
                DisplayName = user.UserName,
                UserIdentifier = applicationUser.Email,
                RoleName = Enum.GetName(typeof(UserRoles), applicationUser.RoleId)
            };
            // Replace FormsAuthenticationTicket with Claims-based approach
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Id),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Email, applicationUser.Email),
                new Claim(ClaimTypes.Role, Enum.GetName(typeof(UserRoles), applicationUser.RoleId))
            };
            var claimsIdentity = new ClaimsIdentity(claims, "TestAuthType");
            var testPrincipal = new ClaimsPrincipal(claimsIdentity);
            // Store user context info in claims
            claims.Add(new Claim("UserContext", userContext.ToString()));



            AccountController controller = new AccountController(userService, userProfileService, goalService, updateService, commentService, followRequestService, followUserService, securityTokenService, userManager, mockMapper.Object, mockWebHostEnvironment.Object);

            principal.SetupGet(x => x.Identity.Name).Returns("adarsh");
            var claimsPrincipal = new Mock<System.Security.Claims.ClaimsPrincipal>();
            claimsPrincipal.Setup(x => x.Identity).Returns(identity.Object);
            controllerContext.SetupGet(x => x.HttpContext.User).Returns(claimsPrincipal.Object);
            controllerContext.SetupGet(p => p.HttpContext.User.Identity.IsAuthenticated).Returns(true);
            controller.ControllerContext = controllerContext.Object;

            contextBase.SetupGet(x => x.Request).Returns(httpRequest.Object);
            contextBase.SetupGet(x => x.Response).Returns(httpResponse.Object);
            genericPrincipal.Setup(x => x.Identity).Returns(identity.Object);

            contextBase.SetupGet(a => a.Response.Cookies).Returns(responseCookies.Object);

            var formsAuthentication = new DefaultFormsAuthentication();



// Mock the authentication in context without using FormsAuthentication
            contextBase.SetupGet(x => x.User).Returns(testPrincipal);

            // Mock the cookie access for testing
            string authCookieValue = "mock-cookie-value"; // Simulated encrypted cookie value
            var mockCookies = new Mock<IRequestCookieCollection>();
            mockCookies.Setup(c => c[".AspNetCore.Cookies"]).Returns(authCookieValue); // Use standard ASP.NET Core cookie name
            contextBase.SetupGet(a => a.Request.Cookies).Returns(mockCookies.Object);

// Create SocialGoalUser directly using the userContext we already set up
            var goalsetterUser = new SocialGoalUser(testPrincipal.Identity.Name, userContext);
            string[] userRoles = { goalsetterUser.RoleName };

            principal.Setup(x => x.Identity).Returns(goalsetterUser);
            IEnumerable<FollowUser> fakeuser = new List<FollowUser> {
            new FollowUser {FollowUserId =1, Accepted = false,FromUserId = "402bd590-fdc7-49ad-9728-40efbfe512ed", ToUserId = "402bd590-fdc7-49ad-9728-40efbfe512ec",},
            new FollowUser {FollowUserId =2, Accepted = false,FromUserId = "402bd590-fdc7-49ad-9728-40efbfe512ee", ToUserId = "402bd590-fdc7-49ad-9728-40efbfe512ec" },

            };
            followUserRepository.Setup(x => x.GetMany(It.IsAny<Expression<Func<FollowUser, bool>>>())).Returns(fakeuser);

            var config = new MapperConfiguration(cfg =>
                cfg.CreateMap<ApplicationUser, FollowersViewModel>()
            );
            var mapper = config.CreateMapper();

            ViewResult result = controller.Followers() as ViewResult;
            ClassicAssert.IsNotNull(result);
            ClassicAssert.IsInstanceOf(typeof(IEnumerable<FollowersViewModel>), result.ViewData.Model, "WrongType");
            var data = result.ViewData.Model as IEnumerable<FollowersViewModel>;
            ClassicAssert.AreEqual(2, data.Count());
        }


        [Test]
        public void Followings_list()
        {
            var userManager = CreateUserManager();
            MemoryUser user = new MemoryUser("adarsh");
            ApplicationUser applicationUser = getApplicationUser();
            var userContext = new UserInfo
            {
                UserId = user.Id,
                DisplayName = user.UserName,
                UserIdentifier = applicationUser.Email,
                RoleName = Enum.GetName(typeof(UserRoles), applicationUser.RoleId)
            };
            // Replace FormsAuthenticationTicket with Claims-based approach
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Id),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Email, applicationUser.Email),
                new Claim(ClaimTypes.Role, Enum.GetName(typeof(UserRoles), applicationUser.RoleId))
            };
            var claimsIdentity = new ClaimsIdentity(claims, "TestAuthType");
            var testPrincipal = new ClaimsPrincipal(claimsIdentity);
            // Store user context info in claims
            claims.Add(new Claim("UserContext", userContext.ToString()));



            AccountController controller = new AccountController(userService, userProfileService, goalService, updateService, commentService, followRequestService, followUserService, securityTokenService, userManager, mockMapper.Object, mockWebHostEnvironment.Object);

            var claimsPrincipal = new Mock<System.Security.Claims.ClaimsPrincipal>();
            claimsPrincipal.Setup(x => x.Identity).Returns(identity.Object);
            controllerContext.SetupGet(x => x.HttpContext.User).Returns(claimsPrincipal.Object);
            controllerContext.SetupGet(p => p.HttpContext.User.Identity.IsAuthenticated).Returns(true);
            controller.ControllerContext = controllerContext.Object;

            contextBase.SetupGet(x => x.Request).Returns(httpRequest.Object);
            contextBase.SetupGet(x => x.Response).Returns(httpResponse.Object);
            genericPrincipal.Setup(x => x.Identity).Returns(identity.Object);

            contextBase.SetupGet(a => a.Response.Cookies).Returns(responseCookies.Object);

            var formsAuthentication = new DefaultFormsAuthentication();



// Mock the authentication in context without using FormsAuthentication
            contextBase.SetupGet(x => x.User).Returns(testPrincipal);

            // Mock the cookie access for testing
            string authCookieValue = "mock-cookie-value"; // Simulated encrypted cookie value
            var mockCookies = new Mock<IRequestCookieCollection>();
            mockCookies.Setup(c => c[".AspNetCore.Cookies"]).Returns(authCookieValue); // Use standard ASP.NET Core cookie name
            contextBase.SetupGet(a => a.Request.Cookies).Returns(mockCookies.Object);

// Create SocialGoalUser directly using the userContext we already set up
            var goalsetterUser = new SocialGoalUser(testPrincipal.Identity.Name, userContext);
            principal.Setup(x => x.Identity).Returns(goalsetterUser);

            IEnumerable<FollowUser> fakeuser = new List<FollowUser> {
            new FollowUser {FollowUserId =1, Accepted = false,FromUserId = "402bd590-fdc7-49ad-9728-40efbfe512ec", ToUserId ="402bd590-fdc7-49ad-9728-40efbfe512ed",},
            new FollowUser {FollowUserId =2, Accepted = false,FromUserId = "402bd590-fdc7-49ad-9728-40efbfe512ec", ToUserId = "402bd590-fdc7-49ad-9728-40efbfe512ee" },

            };
            followUserRepository.Setup(x => x.GetMany(It.IsAny<Expression<Func<FollowUser, bool>>>())).Returns(fakeuser);

            var config = new MapperConfiguration(cfg =>
                cfg.CreateMap<ApplicationUser, FollowersViewModel>()
            );
            var mapper = config.CreateMapper();

            ViewResult result = controller.Followings() as ViewResult;
            ClassicAssert.IsNotNull(result);
            ClassicAssert.IsInstanceOf(typeof(IEnumerable<FollowingViewModel>), result.ViewData.Model, "WrongType");
            var data = result.ViewData.Model as IEnumerable<FollowingViewModel>;
            ClassicAssert.AreEqual(2, data.Count());
        }

        [Test]
        public void Login_Get_View_If_Guid_Is_Null()
        {
            var userManager = CreateUserManager();
            Guid goalIdToken = Guid.NewGuid();
            var emptyQueryMock = new Mock<IQueryCollection>();
            controllerContext.SetupGet(p => p.HttpContext.Request.Query).Returns(emptyQueryMock.Object);
            AccountController controller = new AccountController(userService, userProfileService, goalService, updateService, commentService, followRequestService, followUserService, securityTokenService, userManager, mockMapper.Object, mockWebHostEnvironment.Object);
            controller.ControllerContext = controllerContext.Object;
            ViewResult rslt = controller.Login("abcd") as ViewResult;
            ClassicAssert.IsNotNull(rslt);

        }

        [Test]
        public void Login_Get_View_If_Guid_Is_NotNull()
        {
            var userManager = CreateUserManager();
            //mocking QueryString
            var queryMock = new Mock<IQueryCollection>();
            queryMock.Setup(q => q["guid"]).Returns(new StringValues("got_value"));

            var queryMock1 = new Mock<IQueryCollection>();
            queryMock1.Setup(q => q["reg"]).Returns(new StringValues("value"));
            Guid goalIdToken = Guid.NewGuid();
            // Guid
            controllerContext.SetupGet(p => p.HttpContext.Request.Query).Returns(queryMock.Object);
            //controllerContext.SetupGet(p => p.HttpContext.Request.QueryString).Returns(querystring1);
            controllerContext.SetupGet(p => p.HttpContext.Session).Returns(httpSession.Object);
            AccountController controller = new AccountController(userService, userProfileService, goalService, updateService, commentService, followRequestService, followUserService, securityTokenService, userManager, mockMapper.Object, mockWebHostEnvironment.Object);
            controller.ControllerContext = controllerContext.Object;

            // Using mock HttpContext, HttpRequest, and HttpResponse instead of direct instantiation
            httpRequest.Setup(r => r.Scheme).Returns("http");
            httpRequest.Setup(r => r.Host).Returns(new HostString("localhost"));
            httpRequest.Setup(r => r.Path).Returns(new PathString("/"));
            httpResponse.Setup(r => r.Body).Returns(new MemoryStream());
            contextBase.SetupGet(c => c.Request).Returns(httpRequest.Object);
            contextBase.SetupGet(c => c.Response).Returns(httpResponse.Object);
            var httpContext = contextBase.Object;
// Mock session for ASP.NET Core instead of using HttpSessionStateContainer
            httpSession = new Mock<ISession>();
            contextBase.Setup(x => x.Session).Returns(httpSession.Object);
            httpContext.Items["Session"] = httpSession.Object;

            // HttpContext.Current is not available in ASP.NET Core
            // Using the mocked httpContext directly through controllerContext

            ViewResult rslt = controller.Login("abcd") as ViewResult;
            ClassicAssert.IsNotNull(rslt);
        }

        [Test()]
        public void LoginTest()
        {
            var userManager = CreateUserManager();
            mockSignInManager = new Mock<SignInManager<ApplicationUser>>(
                userManager,
                new Mock<IHttpContextAccessor>().Object,
                new Mock<IUserClaimsPrincipalFactory<ApplicationUser>>().Object,
                new Mock<IOptions<IdentityOptions>>().Object,
                new Mock<ILogger<SignInManager<ApplicationUser>>>().Object,
                new Mock<IAuthenticationSchemeProvider>().Object,
                new Mock<IUserConfirmation<ApplicationUser>>().Object);

            mockSignInManager.Setup(m => m.SignOutAsync()).Returns(Task.CompletedTask);
            mockSignInManager.Setup(m => m.PasswordSignInAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Success);

            AccountController controller = new AccountController(userService, userProfileService, goalService, updateService, commentService, followRequestService, followUserService, securityTokenService, userManager, mockMapper.Object, mockWebHostEnvironment.Object);
            // Set SignInManager on the controller (assuming it has a property to set it)
            // controller.SignInManager = mockSignInManager.Object;

            ApplicationUser applicationUser = getApplicationUser();
            userManager.CreateAsync(applicationUser, "123456");
            var result = controller.Login(new LoginViewModel { Email = "adarsh", Password = "123456", RememberMe = false }, "abcd").Result;
            ClassicAssert.IsNotNull(result);
            var addedUser = userManager.FindByNameAsync("adarsh").Result;
            ClassicAssert.IsNotNull(addedUser);
            ClassicAssert.AreEqual("adarsh", addedUser.UserName);
        }

        [Test]
        public async Task LogOff()
        {
            var userManager = CreateUserManager();
            mockSignInManager = new Mock<SignInManager<ApplicationUser>>(
                userManager,
                new Mock<IHttpContextAccessor>().Object,
                new Mock<IUserClaimsPrincipalFactory<ApplicationUser>>().Object,
                new Mock<IOptions<IdentityOptions>>().Object,
                new Mock<ILogger<SignInManager<ApplicationUser>>>().Object,
                new Mock<IAuthenticationSchemeProvider>().Object,
                new Mock<IUserConfirmation<ApplicationUser>>().Object);

            mockSignInManager.Setup(m => m.SignOutAsync()).Returns(Task.CompletedTask);
            AccountController controller = new AccountController(userService, userProfileService, goalService, updateService, commentService, followRequestService, followUserService, securityTokenService, userManager, mockMapper.Object, mockWebHostEnvironment.Object);
            // Set SignInManager on the controller (assuming it has a property to set it)
            // controller.SignInManager = mockSignInManager.Object;
            // Using mock HttpContext, HttpRequest, and HttpResponse instead of direct instantiation
            httpRequest.Setup(r => r.Scheme).Returns("http");
            httpRequest.Setup(r => r.Host).Returns(new HostString("localhost"));
            httpRequest.Setup(r => r.Path).Returns(new PathString("/"));
            httpResponse.Setup(r => r.Body).Returns(new MemoryStream());
            contextBase.SetupGet(c => c.Request).Returns(httpRequest.Object);
            contextBase.SetupGet(c => c.Response).Returns(httpResponse.Object);
            var httpContext = contextBase.Object;

            // Use ASP.NET Core session mechanism instead of HttpSessionStateContainer
            byte[] dummy = new byte[0];
            httpSession.Setup(s => s.Set(It.IsAny<string>(), It.IsAny<byte[]>()))
                .Callback<string, byte[]>((key, value) => dummy = value);
            httpSession.Setup(s => s.TryGetValue(It.IsAny<string>(), out dummy))
                .Returns(true);
            contextBase.SetupGet(x => x.Session).Returns(httpSession.Object);
            // HttpContext.Current is not available in ASP.NET Core
            // Using the mocked httpContext directly through controllerContext

            var actionResult = await controller.LogOff();
            var result = actionResult as RedirectToRouteResult;
            ClassicAssert.AreEqual("Index", result.RouteValues["action"]);
        }

        [Test]
        public void Register_Get_Returns_View()
        {
            var userManager = CreateUserManager();
            AccountController controller = new AccountController(userService, userProfileService, goalService, updateService, commentService, followRequestService, followUserService, securityTokenService, userManager, mockMapper.Object, mockWebHostEnvironment.Object);
            ViewResult rslt = controller.Register() as ViewResult;
            ClassicAssert.IsNotNull(rslt);
        }

        [Test()]
        public void RegisterTest()
        {
            var userManager = CreateUserManager();
            mockSignInManager = new Mock<SignInManager<ApplicationUser>>(
                userManager,
                new Mock<IHttpContextAccessor>().Object,
                new Mock<IUserClaimsPrincipalFactory<ApplicationUser>>().Object,
                new Mock<IOptions<IdentityOptions>>().Object,
                new Mock<ILogger<SignInManager<ApplicationUser>>>().Object,
                new Mock<IAuthenticationSchemeProvider>().Object,
                new Mock<IUserConfirmation<ApplicationUser>>().Object);

            mockSignInManager.Setup(m => m.SignOutAsync()).Returns(Task.CompletedTask);
            mockSignInManager.Setup(m => m.SignInAsync(It.IsAny<ApplicationUser>(), It.IsAny<bool>(), It.IsAny<string>())).Returns(Task.CompletedTask);
            AccountController controller = new AccountController(userService, userProfileService, goalService, updateService, commentService, followRequestService, followUserService, securityTokenService, userManager, mockMapper.Object, mockWebHostEnvironment.Object);
            // Set SignInManager on the controller (assuming it has a property to set it)
            // controller.SignInManager = mockSignInManager.Object;
            // Using mock HttpContext, HttpRequest, and HttpResponse instead of direct instantiation
            httpRequest.Setup(r => r.Scheme).Returns("http");
            httpRequest.Setup(r => r.Host).Returns(new HostString("localhost"));
            httpRequest.Setup(r => r.Path).Returns(new PathString("/"));
            httpResponse.Setup(r => r.Body).Returns(new MemoryStream());
            contextBase.SetupGet(c => c.Request).Returns(httpRequest.Object);
            contextBase.SetupGet(c => c.Response).Returns(httpResponse.Object);
            var httpContext = contextBase.Object;

            // Use ASP.NET Core session mechanism instead of HttpSessionStateContainer
            byte[] dummy = new byte[0];
            httpSession.Setup(s => s.Set(It.IsAny<string>(), It.IsAny<byte[]>()))
                .Callback<string, byte[]>((key, value) => dummy = value);
            httpSession.Setup(s => s.TryGetValue(It.IsAny<string>(), out dummy))
                .Returns(true);
            contextBase.SetupGet(x => x.Session).Returns(httpSession.Object);
            // HttpContext.Current is not available in ASP.NET Core
            // Using the mocked httpContext directly through controllerContext
            var result =
                controller.Register(new RegisterViewModel
                {
                    UserName = "adarsh",
                    Password = "123456",
                    ConfirmPassword = "123456"
                }).Result;
            ClassicAssert.IsNotNull(result);
            var addedUser = userManager.FindByNameAsync("adarsh").Result;
            ClassicAssert.IsNotNull(addedUser);
            ClassicAssert.AreEqual("adarsh", addedUser.UserName);
        }

        private UserManager<ApplicationUser> CreateUserManager()
        {
            var store = new TestUserStore();
            var optionsMock = new Mock<IOptions<IdentityOptions>>();
            optionsMock.Setup(o => o.Value).Returns(new IdentityOptions());
            var userValidators = new List<IUserValidator<ApplicationUser>>();
            var passwordValidators = new List<IPasswordValidator<ApplicationUser>>();
            var passwordHasher = new PasswordHasher<ApplicationUser>();
            var lookupNormalizer = new UpperInvariantLookupNormalizer();
            var errorDescriber = new IdentityErrorDescriber();
            var serviceProvider = new Mock<IServiceProvider>();
            var loggerMock = new Mock<ILogger<UserManager<ApplicationUser>>>();

            return new UserManager<ApplicationUser>(
                store,
                optionsMock.Object,
                passwordHasher,
                userValidators,
                passwordValidators,
                lookupNormalizer,
                errorDescriber,
                serviceProvider.Object,
                loggerMock.Object);
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
