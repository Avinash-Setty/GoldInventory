using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.UI;
using GoldInventory.Models;
using GoldInventory.ParseWrappers;
using PagedList;

namespace GoldInventory.Controllers
{
    [Authorize]
    public class ItemController : Controller
    {

        public async Task<ActionResult> AutoComplete(string term)
        {
            var items = (await new ItemHelper().GetItemsByName(term)).Take(10).Select(i => new {label = i.Name});
            return Json(items, JsonRequestBehavior.AllowGet);
        }

        //[OutputCache(Duration = 360, VaryByHeader = "X-Requested-With;Accept-Language", Location = OutputCacheLocation.Server)]
        public async Task<ActionResult> Index(string searchTerm = null, int page = 1)
        {
            var items = ((string.IsNullOrWhiteSpace(searchTerm))
                    ? await new ItemHelper().GetAllItems()
                    : await new ItemHelper().GetItemsByName(searchTerm)).ToPagedList(page, 10);

            if (Request.IsAjaxRequest())
            {
                return PartialView("_ItemList", items);
            }

            return View(items);
        }

        // GET: Reviews/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Reviews/Create
        [HttpPost]
        public async Task<ActionResult> Create([Bind(Exclude = "CreatedAt,UpdatedAt,Id")] Item item)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    await new ItemHelper().SaveItem(item);
                    return RedirectToAction("Index");
                }

                return View();
            }
            catch
            {
                return View();
            }
        }

        public ActionResult Edit(string id)
        {
            var item = new ItemHelper().GetItemById(id);
            return View(item);
        }

        // POST: Reviews/Edit/5
        [HttpPost]
        public async Task<ActionResult> Edit(string id, Item item)
        {
            if (ModelState.IsValid)
            {
                await new ItemHelper().SaveItem(item);
                return RedirectToAction("Index");
            }

            return View(item);
        }

        public ActionResult Delete(string id)
        {
            return View();
        }

        // POST: Reviews/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Reviews/Details/5
        public ActionResult Details(string id)
        {
            var item = new ItemHelper().GetItemById(id);
            return View(item);
        }
    }
}