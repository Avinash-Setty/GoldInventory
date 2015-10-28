using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.Identity;
using Parse;

namespace GoldInventory.Models
{
    public class PUser: IUser
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public ParseUser User { get; set; }

        public Company CompanyInfo { get; set; }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(PUserManager manager)
        {
            return new ClaimsIdentity(new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, User.ObjectId),
                new Claim(ClaimTypes.Name, User.Username),
                new Claim(ClaimTypes.Email, User.Email)
            }, DefaultAuthenticationTypes.ApplicationCookie);
        }
    }
}