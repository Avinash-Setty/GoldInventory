using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using GoldInventory.Models;
using GoldInventory.ParseWrappers;
using PagedList;

namespace GoldInventory.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        public async Task<ActionResult> AutoComplete(string term)
        {
            var users = (await new UserHelper().GetCompanyUsersByNameOrEmail(term, 10)).Select(
                i => new {label = i.UserName.Contains(term) ? i.UserName : i.Email});
            return Json(users, JsonRequestBehavior.AllowGet);
        }

        // GET: User
        public async Task<ActionResult> Index(string searchTerm = null, int page = 1)
        {
            ViewBag.HideCreateLink = !await UserUtility.IsUserHasAdminRole();
            var users = (string.IsNullOrWhiteSpace(searchTerm)
                ? await new UserHelper().GetCurrentCompanyUsers()
                : await new UserHelper().GetCompanyUsersByNameOrEmail(searchTerm, 10)).ToPagedList(page, 10);
            if (Request.IsAjaxRequest())
            {
                return PartialView("_UserList", users);
            }

            return View(users);
        }

        // GET: User/Create
        public async Task<ActionResult> Create()
        {
            if (await UserUtility.IsUserHasAdminRole())
                return View(new PUser());
            
            return Content("You do not have enough permission to create User.");
        }

        // POST: User/Create
        [HttpPost]
        public async Task<ActionResult> Create(PUser newUser)
        {
            try
            {
                if (!ModelState.IsValid)
                    return View(newUser);

                var success = await new UserHelper().CreateUsersInCurrentCompany(newUser.Email, newUser.UserName, newUser.Password, newUser.Role);
                if (!success)
                    return View(newUser);

                return RedirectToAction("Index");
            }
            catch
            {
                return View(newUser);
            }
        }

        // GET: User/Edit/5
        public async Task<ActionResult> Edit(string id)
        {
            var user = await new UserHelper().GetUserById(id);
            if (user == null)
                return View();

            return View(user);
        }

        // POST: User/Edit/5
        [HttpPost]
        public async Task<ActionResult> Edit(string id, PUser existingUser)
        {
            try
            {
                var result = await new UserHelper().EditUsersInCurrentCompany(id, existingUser.Email, existingUser.UserName);
                if (result)
                    return RedirectToAction("Index");

                return View(existingUser);
            }
            catch
            {
                return View();
            }
        }

        // GET: User/Delete/5
        public async Task<ActionResult> Delete(string id)
        {
            var user = await new UserHelper().GetUserById(id);
            if (user == null)
                return View();

            return View(user);
        }

        // POST: User/Delete/5
        [HttpPost]
        public async Task<ActionResult> Delete(string id, PUser user)
        {
            try
            {
                var result =await  new UserHelper().DeleteUserInCurrentCompany(id);
                if (!result)
                    return View();

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
