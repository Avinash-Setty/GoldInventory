using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Web.UI;
using GoldInventory.Model;
using GoldInventory.Models;
using GoldInventory.ParseWrappers;
using Microsoft.AspNet.Identity;

namespace GoldInventory.Controllers
{
    [Authorize]
    public class CompanyController : Controller
    {
        [OutputCache(Duration = 520, Location = OutputCacheLocation.Client)]
        public async Task<ActionResult> GetCompanyName()
        {
            var companyInfo = await new CompanyHelper().GetCurrentCompany();
            if (companyInfo == null)
                return Content("Inventory System");

            return Content(companyInfo.Name);
        }

        // GET: Company
        public async Task<ActionResult> Index()
        {
            //HttpContext.GetOwinContext().Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            //RedirectToActionPermanent("LogOff", "Account");
            var companyInfo = await new CompanyHelper().GetCurrentCompany();
            if (companyInfo == null)
            {
                RedirectToActionPermanent("LogOff", "Account");
            }

            return View(companyInfo);
        }

        // GET: Company/Edit/5
        public async Task<ActionResult> Edit()
        {
            var companyInfo = await new CompanyHelper().GetCurrentCompany();
            return View(companyInfo);
        }

        // POST: Company/Edit/5
        [HttpPost]
        public async Task<ActionResult> Edit(Company companyInfo)
        {
            try
            {
                if (!ModelState.IsValid)
                    return View(companyInfo);

                await new CompanyHelper().SaveCompany(companyInfo);
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
