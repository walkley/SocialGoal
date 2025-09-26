using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Mvc.Mailer;
using System.Net.Mail;
using System.Dynamic;

namespace SocialGoal.Web.Mailers
{
    public class UserMailer : MailerBase, IUserMailer
    {
        // Adding a ViewBag property that's accessible in the derived class
        protected dynamic viewBag = new ExpandoObject();

        // Method to get ViewBag data for templates
        protected string GetViewBagData(string key)
        {
            IDictionary<string, object> dict = viewBag as IDictionary<string, object>;
            if (dict != null && dict.ContainsKey(key))
                return dict[key]?.ToString();
            return null;
        }

        public UserMailer() :
            base()
        {
            MasterName = "_Layout";
        }

        public virtual MvcMailMessage Welcome()
        {
            var mailMessage = new MvcMailMessage { Subject = "Welcome" };
            PopulateBody(mailMessage, viewName: "Welcome");
            return mailMessage;
        }


        public virtual MvcMailMessage Invite(string email, Guid groupIdToken)
        {
            var mailMessage = new MvcMailMessage { Subject = "Invite" };
            mailMessage.To.Add(email);

            // Use direct dictionary access rather than ViewBag
            ((IDictionary<string, object>)viewBag)["group"] = "gr:" + groupIdToken;

            PopulateBody(mailMessage, viewName: "Invite");
            return mailMessage;
        }

        public virtual MvcMailMessage Support(string email, Guid goalIdToken)
        {
            var mailMessage = new MvcMailMessage { Subject = "Support My Goal" };
            mailMessage.To.Add(email);
            ((IDictionary<string, object>)viewBag)["goal"] = "go:" + goalIdToken;
            PopulateBody(mailMessage, viewName: "SupportGoal");
            return mailMessage;
        }

        public virtual MvcMailMessage ResetPassword(string email, Guid passwordResetToken)
        {
            var mailMessage = new MvcMailMessage { Subject = "Reset Password" };
            mailMessage.To.Add(email);
            ((IDictionary<string, object>)viewBag)["token"] = "pwreset:" + passwordResetToken;
            PopulateBody(mailMessage,viewName:"PasswordReset");
            return mailMessage;
        }

        public virtual MvcMailMessage InviteNewUser(string email,Guid registrationToken)
        {
            var mailMessage = new MvcMailMessage { Subject = "Invitation to SocialGoal" };
            mailMessage.To.Add(email);
            ((IDictionary<string, object>)viewBag)["token"] = "reg:" + registrationToken;
            PopulateBody(mailMessage, viewName: "InviteNewUser");
            return mailMessage;
        }
    }
}
