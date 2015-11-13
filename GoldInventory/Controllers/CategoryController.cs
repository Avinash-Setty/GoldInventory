using System.Threading.Tasks;
using System.Web.Mvc;
using GoldInventory.Model;
using GoldInventory.ParseWrappers;

namespace GoldInventory.Controllers
{
    [Authorize]
    public class CategoryController : Controller
    {
        // GET: Category
        public async Task<ActionResult> Index()
        {
            var categories = await new ItemCategoryHelper().GetAllItemCategories();
            return View(categories);
        }

        // GET: Category/Details/5
        public ActionResult Details(string id)
        {
            var category = new ItemCategoryHelper().GetItemCategoryById(id);
            return View(category);
        }

        // GET: Category/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Category/Create
        [HttpPost]
        public async Task<ActionResult> Create(ItemCategory newCategory)
        {
            try
            {
                if (!ModelState.IsValid)
                    return View();

                await new ItemCategoryHelper().SaveItemCategory(newCategory);
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Category/Edit/5
        public ActionResult Edit(string id)
        {
            var category = new ItemCategoryHelper().GetItemCategoryById(id);
            return View(category);
        }

        // POST: Category/Edit/5
        [HttpPost]
        public async Task<ActionResult> Edit(string id, ItemCategory editedCategory)
        {
            try
            {
                if (!ModelState.IsValid)
                    return View(editedCategory);

                await new ItemCategoryHelper().SaveItemCategory(editedCategory);
                return RedirectToAction("Index");
            }
            catch
            {
                return View(editedCategory);
            }
        }

        // GET: Category/Delete/5
        public ActionResult Delete(string id)
        {   
            return View(new ItemCategoryHelper().GetItemCategoryById(id));
        }

        // POST: Category/Delete/5
        [HttpPost]
        public async Task<ActionResult> Delete(string id, ItemCategory category)
        {
            try
            {
                await new ItemCategoryHelper().DeleteItemCategory(id);
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
