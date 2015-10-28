using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
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
            itemQuery.WhereEqualTo("CompanyId", ((ParseObject) (currentUser["CompanyId"])));
            var items = await itemQuery.FindAsync();
            if (items == null)
                return new List<Item>();

            var filteredItems = items.Where(item => ((ParseObject)item["CompanyId"]).ObjectId == ((ParseObject) (currentUser["CompanyId"])).ObjectId);
            return filteredItems.Select(i => new Item
            {
                Id = i.ObjectId,
                CreatedAt = i.CreatedAt,
                UpdatedAt = i.UpdatedAt,
                Name = i["Name"]?.ToString(),
                ItemWeight = i.Get<int>("ItemWeight"),
                StoneWeight = i.Get<int>("StoneWeight")
            });
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
            itemObject["CompanyId"] = currentUser["CompanyId"];
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

            if(((ParseObject)(currentUser["CompanyId"]))?.ObjectId != ((ParseObject)(itemObject["CompanyId"]))?.ObjectId)
                return null;

            return new Item
            {
                Id = itemObject.ObjectId,
                Name = itemObject.Get<string>("Name"),
                ItemWeight = itemObject.Get<int>("ItemWeight"),
                StoneWeight = itemObject.Get<int>("StoneWeight"),
                UpdatedAt = itemObject.UpdatedAt,
                CreatedAt = itemObject.CreatedAt
            };
        }

        public async Task<IEnumerable<Item>> GetItemsByName(string name)
        {
            var currentUser = ParseUser.CurrentUser;
            if (currentUser == null)
                return new List<Item>();

            var itemQuery = ParseObject.GetQuery("Item").WhereEqualTo("CompanyId", currentUser["CompanyId"]).WhereContains("Name", name);
            var items = await itemQuery.FindAsync();
            return items?.Select(itemObject => new Item
            {
                Id = itemObject.ObjectId,
                Name = itemObject.Get<string>("Name"),
                ItemWeight = itemObject.Get<int>("ItemWeight"),
                StoneWeight = itemObject.Get<int>("StoneWeight"),
                UpdatedAt = itemObject.UpdatedAt,
                CreatedAt = itemObject.CreatedAt
            });
        }
    }
}