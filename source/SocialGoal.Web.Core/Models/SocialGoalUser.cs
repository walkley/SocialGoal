using System;
using System.Security.Principal;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;


namespace SocialGoal.Web.Core.Models
{
    [Serializable]
    public class SocialGoalUser : IIdentity
    {
        public SocialGoalUser(){}
        public SocialGoalUser(string name, string displayName, string userId)
        {
            this.Name = name;
            this.DisplayName = displayName;
            this.UserId = userId;
        }
        public SocialGoalUser(string name, string displayName, string userId,string roleName)
        {
            this.Name = name;
            this.DisplayName = displayName;
            this.UserId = userId;
            this.RoleName = roleName;
        }
        public SocialGoalUser(string name, UserInfo userInfo)
            : this(name, userInfo.DisplayName, userInfo.UserId,userInfo.RoleName)
        {
            if (userInfo == null) throw new ArgumentNullException("userInfo");
            this.UserId = userInfo.UserId;
        }

        public SocialGoalUser(ClaimsPrincipal principal, string userData)
        {
            if (principal == null) throw new ArgumentNullException("principal");
            this.Name = principal.Identity?.Name ?? string.Empty;
            var userInfo = UserInfo.FromString(userData);
            this.DisplayName = userInfo.DisplayName;
            this.UserId = userInfo.UserId;
            this.RoleName = userInfo.RoleName;
        }

        public string Name { get; private set; }

        public string AuthenticationType
        {
            get { return "GoalSetterForms"; }
        }

        public bool IsAuthenticated
        {
            get { return true; }
        }

        public string DisplayName { get; private set; }
        public string RoleName { get; private set; }
        public string UserId { get; private set; }
    }
}
