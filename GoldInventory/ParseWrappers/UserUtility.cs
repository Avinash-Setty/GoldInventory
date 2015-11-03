using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using Parse;

namespace GoldInventory.ParseWrappers
{
    public static class UserUtility
    {
        public static async Task<ParseUser> GetCurrentParseUser()
        {
            var currentUser = ParseUser.CurrentUser;
            if (currentUser != null) return currentUser;

            var identity = HttpContext.Current.User.Identity as ClaimsIdentity;
            var sessionTokenClaim = identity?.Claims.FirstOrDefault(c => c.Type == "AspNet.Identity.SecurityStamp");
            if (sessionTokenClaim == null)
                return null;

            await ParseUser.BecomeAsync(sessionTokenClaim.Value);
            return ParseUser.CurrentUser;
        }

        public static List<string> GetAllRoles()
        {
            return new List<string>
            {
                UserRole.Admin,
                UserRole.Employee
            };
        }

        public static async Task<bool> IsUserHasAdminRole()
        {
            return (await UserUtility.GetCurrentParseUser())["Role"].ToString() == UserRole.Admin;
        }
    }

    public static class UserRole
    {
        public static readonly string Admin = "admin";
        public static readonly string Employee = "emp";
    }
}