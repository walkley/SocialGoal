using SocialGoal.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;


namespace SocialGoal.Tests.Helpers
{
    public class TestUserStore : IUserStore<ApplicationUser>, IUserLoginStore<ApplicationUser>, IUserRoleStore<ApplicationUser>, IUserClaimStore<ApplicationUser>, IUserPasswordStore<ApplicationUser>, IUserSecurityStampStore<ApplicationUser>
    {
        private Dictionary<string, ApplicationUser> _users;
        private Dictionary<UserLoginInfo, ApplicationUser> _logins = new Dictionary<UserLoginInfo, ApplicationUser>();


        public TestUserStore()
        {
            _users = new Dictionary<string, ApplicationUser>()
            {
                { "TestUser1", new ApplicationUser {FirstName="Sharon", UserName="Sharon", Id="15e8cda1-3ea3-473d-902f-b62aa55816db"}},
                { "TestUser2", new ApplicationUser {FirstName="adarsh", UserName="adarsh", Id="67e6yda1-3ea3-473d-902f-b62er55816db"}},
                { "TestUser3", new ApplicationUser {FirstName="Shiju",  UserName="Shiju",  Id="78e5fda1-3ea3-473d-902f-b62aa86716db"}}
            };

        }


        public Task CreateAsync(ApplicationUser user,string password)
        {
          _users[user.Id] = user;
            return Task.FromResult(0);
        }

        public Task<IdentityResult> CreateAsync(ApplicationUser user, CancellationToken cancellationToken = default)
        {
            _users[user.Id]=user;
            return Task.FromResult(IdentityResult.Success);
        }
        public Task UpdateAsync(ApplicationUser user)
        {
            _users[user.Id] = user;
            return Task.FromResult(0);
        }

        public Task<ApplicationUser> FindByIdAsync(string userId)
        {
            if (_users.ContainsKey(userId))
            {
                return Task.FromResult(_users[userId]);
            }
            return Task.FromResult<ApplicationUser>(null);
        }

        public void Dispose()
        {
        }

        public IQueryable<ApplicationUser> Users
        {
            get
            {
                return _users.Values.AsQueryable();
            }
        }
        public Task<ApplicationUser> FindAsync(string userName, string password)
        {
            var user = new ApplicationUser() { UserName = "adarsh", Id = "402bd590-fdc7-49ad-9728-40efbfe512ec", PasswordHash = "abcd" };
            return Task.FromResult(user);
        }
        public Task<ApplicationUser> FindByNameAsync(string userName)
        {

            foreach (ApplicationUser user in _users.Values)
            {
                if (user.UserName == userName)
                    return Task.FromResult(user);
            }
            return Task.FromResult<ApplicationUser>(null);
        }

        public Task AddLoginAsync(ApplicationUser user, UserLoginInfo login, CancellationToken cancellationToken = default)
        {
            _logins[login] = user;
            return Task.FromResult(0);
        }

        public Task AddLoginAsync(ApplicationUser user, UserLoginInfo login)
        {
            throw new NotImplementedException();
        }
        public Task RemoveLoginAsync(ApplicationUser user, IdentityUserLogin<string> login)
        {
            // In ASP.NET Core Identity, user logins are managed by the store
            // This method is obsolete and replaced by RemoveLoginAsync with loginProvider and providerKey parameters
            throw new NotImplementedException();
        }

        public Task RemoveLoginAsync(ApplicationUser user, UserLoginInfo login)
        {
            throw new NotImplementedException();
        }

        public Task<IList<UserLoginInfo>> GetLoginsAsync(ApplicationUser user)
        {
            throw new NotImplementedException();
        }

        public Task<ApplicationUser> FindAsync(UserLoginInfo login)
        {
            if (_logins.ContainsKey(login))
            {
                return Task.FromResult(_logins[login]);
            }
            return Task.FromResult<ApplicationUser>(null);
        }


        public Task<ApplicationUser> FindByLoginAsync(string loginProvider, string providerKey, CancellationToken cancellationToken = default)
        {
            var login = _logins.Keys.FirstOrDefault(l => l.LoginProvider == loginProvider && l.ProviderKey == providerKey);
            if (login != null && _logins.ContainsKey(login))
            {
                return Task.FromResult(_logins[login]);
            }
            return Task.FromResult<ApplicationUser>(null);
        }

        public Task AddToRoleAsync(ApplicationUser user, string roleName, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(0);
        }

        public Task AddToRoleAsync(ApplicationUser user, string role)
        {
            throw new NotImplementedException();
        }
        public Task RemoveFromRoleAsync(ApplicationUser user, string roleName, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(0);
        }

        public Task RemoveFromRoleAsync(ApplicationUser user, string role)
        {
            throw new NotImplementedException();
        }

public Task<IList<string>> GetRolesAsync(ApplicationUser user, CancellationToken cancellationToken = default)
{
    return Task.FromResult<IList<string>>(new List<string>());
}

public Task<bool> IsInRoleAsync(ApplicationUser user, string roleName, CancellationToken cancellationToken = default)
{
    return Task.FromResult<bool>(true);
}

public Task<IList<ApplicationUser>> GetUsersInRoleAsync(string roleName, CancellationToken cancellationToken = default)
{
    return Task.FromResult<IList<ApplicationUser>>(new List<ApplicationUser>());
}

        public Task<IList<Claim>> GetClaimsAsync(ApplicationUser user, CancellationToken cancellationToken = default)
        {
            return Task.FromResult<IList<Claim>>(new List<Claim>());
        }

        public Task AddClaimAsync(ApplicationUser user, Claim claim, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(0);
        }

        public Task AddClaimAsync(ApplicationUser user, Claim claim)
        {
            throw new NotImplementedException();
        }

        public Task RemoveClaimAsync(ApplicationUser user, Claim claim, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(0);
        }

        public Task RemoveClaimAsync(ApplicationUser user, Claim claim)
        {

            throw new NotImplementedException();
        }

public Task AddClaimsAsync(ApplicationUser user, IEnumerable<Claim> claims, CancellationToken cancellationToken)
{
    return Task.CompletedTask;
}

public Task RemoveClaimsAsync(ApplicationUser user, IEnumerable<Claim> claims, CancellationToken cancellationToken)
{
    return Task.CompletedTask;
}

public Task<IList<ApplicationUser>> GetUsersForClaimAsync(Claim claim, CancellationToken cancellationToken)
{
    return Task.FromResult<IList<ApplicationUser>>(new List<ApplicationUser>());
}

        public Task SetPasswordHashAsync(ApplicationUser user, string passwordHash, CancellationToken cancellationToken = default)
        {
            user.PasswordHash = passwordHash;
            return Task.FromResult(0);
        }

        public Task<string> GetPasswordHashAsync(ApplicationUser user, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(user.PasswordHash);
        }

        public Task SetSecurityStampAsync(ApplicationUser user, string stamp, CancellationToken cancellationToken = default)
        {
            user.SecurityStamp = stamp;
            return Task.FromResult(0);
        }

        public Task<string> GetSecurityStampAsync(ApplicationUser user, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(user.SecurityStamp);
        }

        public Task<IdentityResult> DeleteAsync(ApplicationUser user, CancellationToken cancellationToken = default)
        {
            if (_users.ContainsKey(user.Id))
            {
                _users.Remove(user.Id);
            }
            return Task.FromResult(IdentityResult.Success);
        }

public Task<bool> HasPasswordAsync(ApplicationUser user, CancellationToken cancellationToken = default)
{
    return Task.FromResult(user.PasswordHash != null);
}

public Task<string> GetUserIdAsync(ApplicationUser user, CancellationToken cancellationToken)
{
    return Task.FromResult(user.Id);
}

public Task<string> GetUserNameAsync(ApplicationUser user, CancellationToken cancellationToken)
{
    return Task.FromResult(user.UserName);
}

public Task SetUserNameAsync(ApplicationUser user, string userName, CancellationToken cancellationToken)
{
    user.UserName = userName;
    return Task.FromResult(0);
}

public Task<string> GetNormalizedUserNameAsync(ApplicationUser user, CancellationToken cancellationToken)
{
    return Task.FromResult(user.NormalizedUserName ?? user.UserName.ToUpper());
}

public Task SetNormalizedUserNameAsync(ApplicationUser user, string normalizedName, CancellationToken cancellationToken)
{
    user.NormalizedUserName = normalizedName;
    return Task.FromResult(0);
}

public Task RemoveLoginAsync(ApplicationUser user, string loginProvider, string providerKey, CancellationToken cancellationToken)
{
    var login = _logins.Keys.FirstOrDefault(l => l.LoginProvider == loginProvider && l.ProviderKey == providerKey);
    if (login != null && _logins.ContainsKey(login))
    {
        _logins.Remove(login);
    }
    return Task.FromResult(0);
}

public Task<IList<UserLoginInfo>> GetLoginsAsync(ApplicationUser user, CancellationToken cancellationToken)
{
    IList<UserLoginInfo> logins = _logins.Where(l => l.Value.Id == user.Id).Select(l => l.Key).ToList();
    return Task.FromResult(logins);
}

public Task ReplaceClaimAsync(ApplicationUser user, Claim claim, Claim newClaim, CancellationToken cancellationToken)
{
    return Task.CompletedTask;
}

public Task<ApplicationUser> FindByIdAsync(string userId, CancellationToken cancellationToken)
{
    if (_users.ContainsKey(userId))
    {
        return Task.FromResult(_users[userId]);
    }
    return Task.FromResult<ApplicationUser>(null);
}

public Task<ApplicationUser> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
{
    foreach (ApplicationUser user in _users.Values)
    {
        if ((user.NormalizedUserName ?? user.UserName.ToUpper()) == normalizedUserName)
            return Task.FromResult(user);
    }
    return Task.FromResult<ApplicationUser>(null);
}

public Task<IdentityResult> UpdateAsync(ApplicationUser user, CancellationToken cancellationToken)
{
    _users[user.Id] = user;
    return Task.FromResult(IdentityResult.Success);
}
    }
}
