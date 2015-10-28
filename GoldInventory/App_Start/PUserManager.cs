using System;
using System.Security.Claims;
using System.Threading.Tasks;
using GoldInventory.Models;
using Microsoft.AspNet.Identity;
using Parse;

namespace GoldInventory
{
    public class PUserManager: UserManager<PUser>
    {
        public PUserManager(IUserStore<PUser> store): base(store)
        {
            ClaimsIdentityFactory = new PClaimsIdentityFactory();
        }

        public static PUserManager Create()
        {
            var manager = new PUserManager(new PUserStore());
            manager.UserValidator = new UserValidator<PUser>(manager)
            {
                AllowOnlyAlphanumericUserNames = false,
                RequireUniqueEmail = false
            };

            //manager.PasswordValidator = new PasswordValidator
            //{
            //    RequiredLength = 6,
            //    RequireNonLetterOrDigit = true,
            //    RequireDigit = true,
            //    RequireLowercase = true,
            //    RequireUppercase = true
            //};

            manager.UserLockoutEnabledByDefault = false;
            manager.DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(5);
            manager.MaxFailedAccessAttemptsBeforeLockout = 5;

            return manager;
        }

        public async override Task<bool> CheckPasswordAsync(PUser user, string password)
        {
            var existingUser = await ParseUser.LogInAsync(user.UserName, password);
            user.User = existingUser;
            user.Password = password;
            if (existingUser != null)
                return true;

            return false;
        }
    }
}