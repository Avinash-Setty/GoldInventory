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
            var currentUser = ParseUser.CurrentUser;
            if (currentUser == null)
                return new List<PUser>();

            var userQuery = new ParseQuery<ParseUser>();
            var currentCompanyUsers = await userQuery.FindAsync();
            if(currentCompanyUsers == null)
                return new List<PUser>();

            return currentCompanyUsers.Where(
                    user => ((ParseObject) user["CompanyId"]).ObjectId == ((ParseObject) currentUser["CompanyId"]).ObjectId)
                    .Select(user => new PUser
                    {
                        Id = user.ObjectId,
                        Email = user["email"]?.ToString(),
                        UserName = user["username"]?.ToString()
                    });
        }
    }
}