using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using GoldInventory.ParseWrappers;
using Microsoft.AspNet.Identity;
using Parse;

namespace GoldInventory.Models
{
    public class PUser: IUser
    {   
        public string Id { get; set; }

        [Required]
        [Display(Name = "UserName")]
        public string UserName { get; set; }

        [Required]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Confirm Password")]
        public string ConfirmPassword { get; set; }

        [Required]
        [Display(Name = "Email")]
        [EmailAddress]
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

        [Required]
        [Display(Name = "Role")]
        public string Role { get; set; }

        public IEnumerable<SelectListItem> AvailableRoles => new List<SelectListItem>
        {
            new SelectListItem
            {
                Text = "Admin",
                Value = UserRole.Admin
            },
            new SelectListItem
            {
                Text = "Employee",
                Value = UserRole.Employee
            }
        };
    }
}