using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GoldInventory.Model;
using Parse;

namespace GoldInventory.ParseWrappers
{
    public class ItemCategoryHelper
    {
        public ParseObject RawItemCategory { get; set; }

        public async Task<IEnumerable<ItemCategory>> GetAllItems()
        {
            var currentUser = await UserUtility.GetCurrentParseUser();
            if (currentUser == null)
                return new List<ItemCategory>(); ;

            var itemQuery = ParseObject.GetQuery("Category");
            itemQuery.WhereEqualTo("CompanyId", currentUser["CompanyId"].ToString());
            var items = await itemQuery.FindAsync();
            if (items == null)
                return new List<ItemCategory>();

            return items.Where(c => c["CompanyId"].ToString() == currentUser["CompanyId"].ToString()).Select(i => new ItemCategory
            {
                Id = i.ObjectId,
                CreatedAt = i.CreatedAt,
                UpdatedAt = i.UpdatedAt,
                Name = i["Name"]?.ToString(),
                Description = i["Description"]?.ToString()
            });
        }

        public async Task SaveItemCategory(ItemCategory category)
        {
            var currentUser = await UserUtility.GetCurrentParseUser();
            if (currentUser == null)
                return;

            var itemObject = new ParseObject("Category");
            if (!string.IsNullOrEmpty(category.Id))
                itemObject.ObjectId = category.Id;

            itemObject["Name"] = category.Name;
            itemObject["Description"] = category.Description;
            itemObject["CompanyId"] = currentUser["CompanyId"].ToString();
            RawItemCategory = itemObject;
            await itemObject.SaveAsync();
        }

        public ItemCategory GetItemCategoryById(string id)
        {
            var currentUser = ParseUser.CurrentUser;
            if (currentUser == null)
                return null;

            var itemQuery = ParseObject.GetQuery("Category");
            var itemObject = itemQuery.GetAsync(id).Result;
            if (itemObject == null)
                return null;

            if (currentUser["CompanyId"].ToString() != itemObject["CompanyId"]?.ToString())
                return null;

            RawItemCategory = itemObject;
            return new ItemCategory
            {
                Id = itemObject.ObjectId,
                Name = itemObject.Get<string>("Name"),
                Description = itemObject.Get<string>("Description"),
                UpdatedAt = itemObject.UpdatedAt,
                CreatedAt = itemObject.CreatedAt
            };
        }

        public async Task<bool> DeleteItemCategory(string id)
        {
            var category = GetItemCategoryById(id);
            if (category == null)
                return false;

            await RawItemCategory.DeleteAsync();
            return true;
        }
    }
}