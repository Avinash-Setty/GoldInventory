using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GoldInventory.Models;
using Parse;

namespace GoldInventory.ParseWrappers
{
    public class UserHelper
    {
        public async Task<IEnumerable<PUser>> GetCompanyUsersByNameOrEmail(string searchTerm, int resultsCount)
        {
            var currentUser = await UserUtility.GetCurrentParseUser();
            if (currentUser == null || string.IsNullOrWhiteSpace(searchTerm) || resultsCount <= 0)
                return new List<PUser>();

            var usernameQuery = from user in ParseUser.Query
                where user.Get<string>("CompanyId").Equals(currentUser["CompanyId"].ToString())
                      && user.Get<string>("username").Contains(searchTerm)
                select user;

            var emailQuery = from user in ParseUser.Query
                             where user.Get<string>("CompanyId").Equals(currentUser["CompanyId"].ToString())
                                   && user.Get<string>("email").Contains(searchTerm)
                             select user;

            var allUsers = await GetCurrentCompanyUsers(usernameQuery.Or(emailQuery));
            return allUsers;
        }

        public async Task<PUser> GetCompanyUserByEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return null;

            var emailQuery = from user in ParseUser.Query
                where user.Email.Equals(email)
                select user;
            var users = (await GetCurrentCompanyUsers(emailQuery))?.ToList();
            if (users == null || !users.Any())
                return null;

            return users.FirstOrDefault();
        } 

        public async Task<IEnumerable<PUser>> GetCurrentCompanyUsers(ParseQuery<ParseUser> query = null)
        {
            IEnumerable<ParseUser> currentCompanyUsers;
            if (query == null)
            {
                var currentUser = await UserUtility.GetCurrentParseUser();
                if (currentUser == null)
                    return new List<PUser>();

                currentCompanyUsers = await ParseUser.Query.WhereEqualTo("CompanyId", currentUser["CompanyId"].ToString()).FindAsync();
            }
            else
                currentCompanyUsers = await query.FindAsync();

            if (currentCompanyUsers == null)
                return new List<PUser>();

            return currentCompanyUsers
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