using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GoldInventory.Models;
using Parse;
using WebGrease.Css.Extensions;

namespace GoldInventory.ParseWrappers
{
    public class SupportedAttributeTypes
    {
        public static readonly string String = "string";
        public static readonly string Number = "number";
    }
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

            await AssociateAllItems(filteredItems);
            return filteredItems;
        }

        private async Task AssociateAllItems(List<Item> items)
        {
            var allAttrs = await new AttributeHelper().GetAllAttributes();
            var allItemAttrs = (await new ItemAttributeHelper().GetAllItemAttributesByItemIds(items.Select(i => i.Id))).ToList();
            allItemAttrs.ForEach(i => i.AttributeName = allAttrs.FirstOrDefault(a => a.Id == i.AttributeId)?.Name ?? "Error");
            items.ForEach(i => i.AssociatedAttributes = allItemAttrs.Where(a => a.ItemId == i.Id));
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

            item.AssociatedAttributes.ForEach(attr => attr.ItemId = itemObject.ObjectId);
            await new ItemAttributeHelper().SaveAllItemAttributes(item.AssociatedAttributes);
        }

        public async Task<Item> GetItemById(string id)
        {
            var itemObject = await GetRawItemObjectById(id);
            var category = new ItemCategoryHelper().GetItemCategoryById(itemObject.Get<string>("CategoryId"));
            var item = new Item
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
            await AssociateAllItems(new List<Item> {item});
            return item;
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