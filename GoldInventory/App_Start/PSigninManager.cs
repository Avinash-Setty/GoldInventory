using System;
using System.Threading.Tasks;
using GoldInventory.Models;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Parse;

namespace GoldInventory
{
    public class PSignInManager : SignInManager<PUser, string>
    {
        public PSignInManager(PUserManager userManager, IAuthenticationManager authenticationManager)
        : base(userManager, authenticationManager)
        {
        }

        public static PSignInManager Create(IdentityFactoryOptions<PSignInManager> options, IOwinContext context)
        {
            return new PSignInManager(context.GetUserManager<PUserManager>(), context.Authentication);
        }

        public async override Task SignInAsync(PUser user, bool isPersistent, bool rememberBrowser)
        {
            if (!user.User.IsAuthenticated)
                await ParseUser.LogInAsync(user.UserName, user.Password);

            try
            {
                await base.SignInAsync(user, isPersistent, rememberBrowser);
            }
            catch (Exception e)
            {   
                throw e;
            }
        }

        public async override Task<SignInStatus> PasswordSignInAsync(string userName, string password, bool isPersistent, bool shouldLockout)
        {
            try
            {
                return await base.PasswordSignInAsync(userName, password, isPersistent, shouldLockout);
            }
            catch (Exception e)
            {
                return SignInStatus.Failure;
            }
        }
    }
}