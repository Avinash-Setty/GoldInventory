using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using GoldInventory.Model;
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
            var items = (await new ItemHelper().GetItemsByName(term, false)).Take(10).Select(i => new { label = i.Name });
            return Json(items, JsonRequestBehavior.AllowGet);
        }

        //[OutputCache(Duration = 360, VaryByHeader = "X-Requested-With;Accept-Language", Location = OutputCacheLocation.Server)]
        public async Task<ActionResult> Index(string searchTerm = null, int page = 1)
        {
            var items = ((string.IsNullOrWhiteSpace(searchTerm))
                    ? await new ItemHelper().GetAllItems()
                    : await new ItemHelper().GetItemsByName(searchTerm)).ToPagedList(page, 10);
            var associatedAttributes = await new ItemAttributeHelper().GetBareAttributes();
            ViewData.Add("AttributeNames", associatedAttributes.Select(a => a.AttributeName).OrderBy(a => a));
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
                AvailableCategories = await new ItemCategoryHelper().GetAllItemCategories(),
                AssociatedAttributes = await new ItemAttributeHelper().GetBareAttributes()
            };
            return View(newItem);
        }

        // POST: Reviews/Create
        [HttpPost]
        public async Task<ActionResult> Create([Bind(Exclude = "CreatedAt,UpdatedAt,Id")] FormCollection collection)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var attributes = (await new ItemAttributeHelper().GetBareAttributes()).ToList();
                    attributes.ForEach(attr => attr.Value = collection[attr.AttributeId]);
                    var newItem = new Item
                    {
                        AssociatedAttributes = attributes,
                        CategoryId = collection["CategoryId"],
                        Name = collection["Name"],
                        PhotoStream = (Request.Files.Count > 0) ? Request.Files[0]?.InputStream : null,
                        PhotoContentType = (Request.Files.Count > 0) ? Request.Files[0]?.ContentType : null
                    };
                    await new ItemHelper().SaveItem(newItem);
                    return RedirectToAction("Index");
                }

                var placeHolder = new Item
                {
                    AvailableCategories = await new ItemCategoryHelper().GetAllItemCategories(),
                    AssociatedAttributes = await new ItemAttributeHelper().GetBareAttributes()
                };
                return View(placeHolder);
            }
            catch
            {
                var placeHolder = new Item
                {
                    AvailableCategories = await new ItemCategoryHelper().GetAllItemCategories(),
                    AssociatedAttributes = await new ItemAttributeHelper().GetBareAttributes()
                };
                return View(placeHolder);
            }
        }

        public async Task<ActionResult> Edit(string id)
        {
            var item = await new ItemHelper().GetItemById(id);
            item.AvailableCategories = await new ItemCategoryHelper().GetAllItemCategories();
            return View(item);
        }

        // POST: Reviews/Edit/5
        [HttpPost]
        public async Task<ActionResult> Edit(string id, FormCollection collection)
        {
            if (ModelState.IsValid)
            {
                var attributes = (await new ItemAttributeHelper().GetBareAttributes()).ToList();
                attributes.ForEach(attr =>
                {
                    attr.Value = collection[attr.AttributeId];
                    attr.ItemId = id;
                    attr.Id = collection[attr.AttributeId + attr.ItemId];
                });
                var newItem = new Item
                {
                    Id = id,
                    AssociatedAttributes = attributes,
                    CategoryId = collection["CategoryId"],
                    Name = collection["Name"],
                    PhotoStream = (Request.Files.Count > 0) ? Request.Files[0]?.InputStream : null,
                    PhotoContentType = (Request.Files.Count > 0) ? Request.Files[0]?.ContentType : null
                };
                await new ItemHelper().SaveItem(newItem);
                return RedirectToAction("Index");
            }

            var item = await new ItemHelper().GetItemById(id);
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