using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using GoldInventory.Models;
using Microsoft.AspNet.Identity;
using Parse;

namespace GoldInventory
{
    public class PClaimsIdentityFactory: IClaimsIdentityFactory<PUser, string>
    {
        /// <summary>
        /// Claim type used for role claims
        /// </summary>
        public string RoleClaimType { get; set; }

        /// <summary>
        /// Claim type used for the user name
        /// </summary>
        public string UserNameClaimType { get; set; }

        /// <summary>
        /// Claim type used for the user id
        /// </summary>
        public string UserIdClaimType { get; set; }

        /// <summary>
        /// Claim type used for the user security stamp
        /// </summary>
        public string SecurityStampClaimType { get; set; }

        public PClaimsIdentityFactory()
        {
            RoleClaimType = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role";
            UserIdClaimType = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier";
            UserNameClaimType = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name";
            SecurityStampClaimType = "AspNet.Identity.SecurityStamp";
        }

        public async Task<ClaimsIdentity> CreateAsync(UserManager<PUser, string> manager, PUser user, string authenticationType)
        {
            if (manager == null)
                throw new ArgumentNullException("manager");
            if (user == null)
                throw new ArgumentNullException("user");

            var id = new ClaimsIdentity(authenticationType, UserNameClaimType, RoleClaimType);
            id.AddClaim(new Claim(SecurityStampClaimType, (await ParseSession.GetCurrentSessionAsync()).SessionToken, "http://www.w3.org/2001/XMLSchema#string"));
            id.AddClaim(new Claim(UserIdClaimType, user.User.ObjectId, "http://www.w3.org/2001/XMLSchema#string"));
            id.AddClaim(new Claim(UserNameClaimType, user.UserName, "http://www.w3.org/2001/XMLSchema#string"));
            id.AddClaim(new Claim("http://schemas.microsoft.com/accesscontrolservice/2010/07/claims/identityprovider", "ASP.NET Identity", "http://www.w3.org/2001/XMLSchema#string"));
            if (manager.SupportsUserSecurityStamp)
            {
                //ClaimsIdentity claimsIdentity1 = id;
                //string securityStampClaimType = SecurityStampClaimType;
                //ClaimsIdentity claimsIdentity2 = claimsIdentity1;
                //string str = await manager.GetSecurityStampAsync(user.User.ObjectId).ConfigureAwait(false);
                //Claim claim = new Claim(securityStampClaimType, str ?? Guid.NewGuid().ToString());
                //claimsIdentity2.AddClaim(claim);
            }
            if (manager.SupportsUserRole)
            {
                IList<string> roles = await manager.GetRolesAsync(user.Id).ConfigureAwait(false);
                foreach (string str in roles)
                    id.AddClaim(new Claim(RoleClaimType, str, "http://www.w3.org/2001/XMLSchema#string"));
            }
            if (manager.SupportsUserClaim)
                id.AddClaims(await manager.GetClaimsAsync(user.Id).ConfigureAwait(false));
            return id;
        }
    }
}