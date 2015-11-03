using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GoldInventory.Models;
using Parse;

namespace GoldInventory.ParseWrappers
{
    public class UserHelper
    {
        public async Task<IEnumerable<PUser>> GetCurrentCompanyUsers()
        {
            var currentUser = await UserUtility.GetCurrentParseUser();
            if (currentUser == null)
                return new List<PUser>();
            
            var currentCompanyUsers = await ParseUser.Query.WhereEqualTo("CompanyId", currentUser["CompanyId"].ToString()).FindAsync();
            if (currentCompanyUsers == null)
                return new List<PUser>();

            return currentCompanyUsers.Where(u => u.ObjectId != currentUser.ObjectId)
                .Select(user => new PUser
                {
                    Id = user.ObjectId,
                    Email = user["email"]?.ToString(),
                    UserName = user["username"]?.ToString(),
                    Role = (user["Role"]?.ToString() == UserRole.Admin) ? "Admin" : "Employee"
                });
        }

        public async Task<bool> CreateUsersInCurrentCompany(string email, string username, string password, string role)
        {
            var currentUser = await UserUtility.GetCurrentParseUser();
            if (currentUser == null)
                return false;

            if (currentUser["Role"].ToString() != UserRole.Admin)
                return false;

            var newUser = new ParseUser
            {
                Email = email,
                Username = username,
                Password = password,
                ["CompanyId"] = currentUser["CompanyId"].ToString(),
                ["PasswordFailures"] = 0,
                ["Role"] = role
            };

            //TODO: add role to this user and role should be non-admin

            await newUser.SignUpAsync();

            // We are logging out the user because sign up creates the user and logs in
            ParseUser.LogOut();
            return true;
        }

        public async Task<bool> EditUsersInCurrentCompany(string id, string email, string username)
        {
            var user = await GetUserInRawFormatById(id);
            if (user == null)
                return false;

            
            user.Email = email;
            user.Username = username;
            await user.SaveAsync();
            return true;
        }

        public async Task<bool> DeleteUserInCurrentCompany(string id)
        {
            var user = await GetUserInRawFormatById(id);
            if (user == null)
                return false;
            //TODO: Also add logic to check whether current user is admin

            await user.DeleteAsync();
            return true;
        }

        private async Task<ParseUser> GetUserInRawFormatById(string id)
        {
            var currentUser = await UserUtility.GetCurrentParseUser();
            if (currentUser == null)
                return null;

            var user = await ParseUser.Query.GetAsync(id);
            if (user["CompanyId"].ToString() != currentUser["CompanyId"].ToString())
                return null;

            return user;
        }

        public async Task<PUser> GetUserById(string id)
        {
            var user = await GetUserInRawFormatById(id);
            if (user == null)
                return null;

            return new PUser
            {
                Id = user.ObjectId,
                Email = user["email"]?.ToString(),
                UserName = user["username"]?.ToString()
            };
        }
    }
}