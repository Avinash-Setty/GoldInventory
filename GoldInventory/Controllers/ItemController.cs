using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
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
        public async Task<ActionResult> Create()
        {
            var newItem = new Item
            {
                AvailableCategories = await new ItemCategoryHelper().GetAllItems()
            };
            return View(newItem);
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

        public async Task<ActionResult> Edit(string id)
        {
            var item = await new ItemHelper().GetItemById(id);
            item.AvailableCategories = await new ItemCategoryHelper().GetAllItems();
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

        public async Task<ActionResult> Delete(string id)
        {
            var item = await new ItemHelper().GetItemById(id);
            return View(item);
        }

        // POST: Reviews/Delete/5
        [HttpPost]
        public async Task<ActionResult> Delete(string id, FormCollection collection)
        {
            try
            {
                var result = await new ItemHelper().DeleteItemById(id);
                if (result)
                    return RedirectToAction("Index");

                return View();
            }
            catch
            {
                return View();
            }
        }

        // GET: Reviews/Details/5
        public async Task<ActionResult> Details(string id)
        {
            var item = await new ItemHelper().GetItemById(id);
            return View(item);
        }
    }
}