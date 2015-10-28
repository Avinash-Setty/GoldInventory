using System;
using System.Linq;
using System.Threading.Tasks;
using GoldInventory.Models;
using GoldInventory.ParseWrappers;
using Microsoft.AspNet.Identity;
using Parse;

namespace GoldInventory
{
    public class PUserStore : IUserStore<PUser>, IUserTwoFactorStore<PUser, string>, IUserLockoutStore<PUser, string>
    {
        public IUserStore<PUser> Store { get; set; }
        public PUserStore(IUserStore<PUser> store)
        {
            if (store == null)
                throw new ArgumentNullException("store");
            Store = store;
        }

        public PUserStore()
        {
            ParseClient.Initialize("WinpiYKuQBxG6jGeBtwX5rkQBclHyolE2LyQSASt",
                "9nJ6TOnG59z07qUpiQWIsfhML5flFxt64VpDppPs");
        }

        public void Dispose()
        {

        }

        public async Task CreateAsync(PUser user)
        {
            user.User["PasswordFailures"] = 0;
            user.User["CompanyId"] = (ParseObject)new CompanyHelper().SaveCompany(user.CompanyInfo).Result;
            await user.User.SignUpAsync();
        }

        public async Task UpdateAsync(PUser user)
        {
            await user.User.SaveAsync();
        }

        public async Task DeleteAsync(PUser user)
        {
            await user.User.DeleteAsync();
        }

        public async Task<PUser> FindByIdAsync(string userId)
        {
            var user = await ParseUser.Query.GetAsync(userId);
            if (user == null)
                return null;

            return new PUser
            {
                Id = user.ObjectId,
                User = user,
                UserName = user.Username
            };
        }

        public async Task<PUser> FindByNameAsync(string userName)
        {
            var users = await (from u in ParseUser.Query
                               where u.Get<string>("username") == userName
                               select u).FindAsync();
            var user = users.FirstOrDefault();
            if (user == null)
                return null;

            return new PUser
            {
                Id = user.ObjectId,
                User = user,
                UserName = user.Username
            };
        }

        public Task<DateTimeOffset> GetLockoutEndDateAsync(PUser user)
        {
            if (user.User == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            int failures;
            if (int.TryParse(user.User["PasswordFailures"].ToString(), out failures) && failures >= 5)
                return Task.FromResult(DateTimeOffset.UtcNow.AddDays(1000));

            return Task.FromResult(DateTimeOffset.UtcNow.Subtract(new TimeSpan(1, 0, 0, 0)));
        }

        public Task SetLockoutEndDateAsync(PUser user, DateTimeOffset lockoutEnd)
        {
            if (user.User == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            int failures;
            if (int.TryParse(user.User["PasswordFailures"].ToString(), out failures) && failures >= 5)
                return Task.FromResult(DateTimeOffset.UtcNow.AddDays(1000));

            return Task.FromResult(DateTimeOffset.UtcNow.Subtract(new TimeSpan(1, 0, 0, 0)));
        }

        public Task<int> IncrementAccessFailedCountAsync(PUser user)
        {
            if (user.User == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            int failures;
            if (int.TryParse(user.User["PasswordFailures"].ToString(), out failures))
            {
                user.User["PasswordFailures"] = failures + 1;
                user.User.SaveAsync();
                return Task.FromResult(failures + 1);
            }

            return Task.FromResult(1);
        }

        public Task ResetAccessFailedCountAsync(PUser user)
        {
            if (user.User == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            user.User["PasswordFailures"] = 0;
            user.User.SaveAsync();
            return Task.FromResult(0);
        }

        public Task<int> GetAccessFailedCountAsync(PUser user)
        {
            int failures;
            return Task.FromResult(int.TryParse(user.User["PasswordFailures"].ToString(), out failures) ? failures : 0);
        }

        public Task<bool> GetLockoutEnabledAsync(PUser user)
        {
            return Task.FromResult(false);
        }

        public Task SetLockoutEnabledAsync(PUser user, bool enabled)
        {
            if (user.User == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            int failures;
            if (int.TryParse(user.User["PasswordFailures"].ToString(), out failures) && failures >= 5)
                return Task.FromResult(DateTimeOffset.UtcNow.AddDays(1000));

            return Task.FromResult(0);
        }

        public Task SetTwoFactorEnabledAsync(PUser user, bool enabled)
        {
            throw new NotImplementedException();
        }

        public Task<bool> GetTwoFactorEnabledAsync(PUser user)
        {
            return Task.FromResult(false);
        }
    }
}