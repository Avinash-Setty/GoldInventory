using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using GoldInventory.Models;
using Parse;

namespace GoldInventory.ParseWrappers
{
    public class ItemHelper
    {
        public async Task<IEnumerable<Item>> GetAllItems()
        {
            var currentUser = ParseUser.CurrentUser;
            if (currentUser == null)
                return new List<Item>(); ;

            var itemQuery = ParseObject.GetQuery("Item");
            itemQuery.WhereEqualTo("CompanyId", ((ParseObject) (currentUser["CompanyId"])).ObjectId);
            var items = await itemQuery.FindAsync();
            if (items == null)
                return new List<Item>();

            var allCategories = await new ItemCategoryHelper().GetAllItems();
            var filteredItems = new List<Item>();
            foreach (var item in items)
            {
                if(!item.ContainsKey("CompanyId") || !item.ContainsKey("CategoryId"))
                    continue;

                if(item["CompanyId"].ToString() != ((ParseObject)(currentUser["CompanyId"])).ObjectId)
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
            var currentUser = ParseUser.CurrentUser;
            if (currentUser == null)
                return;

            var itemObject = new ParseObject("Item");
            if (!string.IsNullOrEmpty(item.Id))
                itemObject.ObjectId = item.Id;

            itemObject["Name"] = item.Name;
            itemObject["ItemWeight"] = item.ItemWeight;
            itemObject["StoneWeight"] = item.StoneWeight;
            itemObject["CategoryId"] = item.CategoryId;
            itemObject["CompanyId"] = ((ParseObject)currentUser["CompanyId"]).ObjectId;
            await itemObject.SaveAsync();
        }

        public Item GetItemById(string id)
        {
            var currentUser = ParseUser.CurrentUser;
            if (currentUser == null)
                return null;

            var itemQuery = ParseObject.GetQuery("Item");
            var itemObject = itemQuery.GetAsync(id).Result;
            if (itemObject == null)
                return null;

            if(((ParseObject)(currentUser["CompanyId"]))?.ObjectId != itemObject["CompanyId"].ToString())
                return null;

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

        public async Task<IEnumerable<Item>> GetItemsByName(string name)
        {
            var currentUser = ParseUser.CurrentUser;
            if (currentUser == null)
                return new List<Item>();

            var itemQuery = ParseObject.GetQuery("Item").WhereEqualTo("CompanyId", ((ParseObject)currentUser["CompanyId"]).ObjectId).WhereContains("Name", name);
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