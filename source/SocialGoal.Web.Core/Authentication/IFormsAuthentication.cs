using Microsoft.AspNetCore.Http;
using System;

namespace SocialGoal.Web.Core.Authentication
{
    public class AuthenticationTicket
    {
        public string Name { get; set; }
        public bool IsPersistent { get; set; }
        public string UserData { get; set; }
        public DateTime Expiration { get; set; }
        public DateTime IssueDate { get; set; }
        public int Version { get; set; }
        public string CookiePath { get; set; }
    }

    // Renamed to avoid duplicate definition
    public interface IFormsAuthenticationService
    {
        void Signout();
        void SetAuthCookie(HttpContext httpContext, AuthenticationTicket authenticationTicket);
        AuthenticationTicket Decrypt(string encryptedTicket);
    }
}
