using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GoldInventory.Models;
using Parse;

namespace GoldInventory.ParseWrappers
{
    public class ItemHelper
    {
        public async Task<IEnumerable<Item>> GetAllItems()
        {
            var currentUser = await UserUtility.GetCurrentParseUser();
            if (currentUser == null)
                return new List<Item>();

            var itemQuery = ParseObject.GetQuery("Item");
            itemQuery.WhereEqualTo("CompanyId", currentUser["CompanyId"].ToString());
            var items = await itemQuery.FindAsync();
            if (items == null)
                return new List<Item>();

            var allCategories = (await new ItemCategoryHelper().GetAllItems()).ToList();
            var filteredItems = new List<Item>();
            foreach (var item in items)
            {
                if (!item.ContainsKey("CompanyId") || !item.ContainsKey("CategoryId"))
                    continue;

                if (item["CompanyId"].ToString() != currentUser["CompanyId"].ToString())
                    continue;

                filteredItems.Add(new Item
                {
                    Id = item.ObjectId,
                    CreatedAt = item.CreatedAt,
                    UpdatedAt = item.UpdatedAt,
                    Name = item["Name"]?.ToString(),
                    ItemWeight = item.Get<int>("ItemWeight"),
                    StoneWeight = item.Get<int>("StoneWeight"),
                    CategoryName = allCategories.FirstOrDefault(c => c.Id == item.Get<string>("CategoryId"))?.Name
                });
            }

            return filteredItems;
        }

        public async Task SaveItem(Item item)
        {
            var currentUser = await UserUtility.GetCurrentParseUser();
            if (currentUser == null)
                return;

            var itemObject = new ParseObject("Item");
            if (!string.IsNullOrEmpty(item.Id))
                itemObject.ObjectId = item.Id;

            itemObject["Name"] = item.Name;
            itemObject["ItemWeight"] = item.ItemWeight;
            itemObject["StoneWeight"] = item.StoneWeight;
            itemObject["CategoryId"] = item.CategoryId;
            itemObject["CompanyId"] = currentUser["CompanyId"].ToString();
            await itemObject.SaveAsync();
        }

        public async Task<Item> GetItemById(string id)
        {
            var itemObject = await GetRawItemObjectById(id);
            var category = new ItemCategoryHelper().GetItemCategoryById(itemObject.Get<string>("CategoryId"));
            return new Item
            {
                Id = itemObject.ObjectId,
                Name = itemObject.Get<string>("Name"),
                ItemWeight = itemObject.Get<int>("ItemWeight"),
                StoneWeight = itemObject.Get<int>("StoneWeight"),
                CategoryName = category?.Name,
                CategoryId = category?.Id,
                UpdatedAt = itemObject.UpdatedAt,
                CreatedAt = itemObject.CreatedAt
            };
        }

        private async Task<ParseObject> GetRawItemObjectById(string id)
        {
            var currentUser = await UserUtility.GetCurrentParseUser();
            if (currentUser == null)
                return null;

            var itemQuery = ParseObject.GetQuery("Item");
            var itemObject = await itemQuery.GetAsync(id);
            if (currentUser["CompanyId"].ToString() != itemObject["CompanyId"].ToString())
                return null;

            return itemObject;
        }

        public async Task<bool> DeleteItemById(string id)
        {
            var rawItem = await GetRawItemObjectById(id);
            if (rawItem == null)
                return false;

            await rawItem.DeleteAsync();
            return true;
        } 

        public async Task<IEnumerable<Item>> GetItemsByName(string name)
        {
            var currentUser = await UserUtility.GetCurrentParseUser();
            if (currentUser == null)
                return new List<Item>();

            var itemQuery = ParseObject.GetQuery("Item").WhereEqualTo("CompanyId", currentUser["CompanyId"].ToString()).WhereContains("Name", name);
            var items = await itemQuery.FindAsync();
            return items?.Select(itemObject => new Item
            {
                Id = itemObject.ObjectId,
                Name = itemObject.Get<string>("Name"),
                ItemWeight = itemObject.Get<int>("ItemWeight"),
                StoneWeight = itemObject.Get<int>("StoneWeight"),
                CategoryId = itemObject.Get<string>("CategoryId"),
                UpdatedAt = itemObject.UpdatedAt,
                CreatedAt = itemObject.CreatedAt
            });
        }
    }
}