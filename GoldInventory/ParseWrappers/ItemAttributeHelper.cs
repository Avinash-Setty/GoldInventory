using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GoldInventory.Model;
using Parse;

namespace GoldInventory.ParseWrappers
{
    public class ItemAttributeHelper
    {
        public async Task<IEnumerable<ItemAttribute>> GetAllItemAttributesByItemIds(IEnumerable<string> itemIds)
        {
            var attrQuery = ParseObject.GetQuery("ItemAttribute");
            attrQuery.WhereContainedIn("ItemId", itemIds);
            var attrs = await attrQuery.FindAsync();
            if (attrs == null)
                return new List<ItemAttribute>();

            var filteredItemAttrs = new List<ItemAttribute>();
            foreach (var attr in attrs)
            {
                filteredItemAttrs.Add(new ItemAttribute
                {
                    Id = attr.ObjectId,
                    CreatedAt = attr.CreatedAt,
                    UpdatedAt = attr.UpdatedAt,
                    ItemId = attr["ItemId"]?.ToString(),
                    AttributeId = attr["AttributeId"]?.ToString(),
                    Value = attr["Value"]?.ToString(),
                });
            }

            return filteredItemAttrs;
        }

        public void SaveAllItemAttributes(IEnumerable<ItemAttribute> attributes)
        {
            var allTasks = attributes.Select(SaveItemAttribute).ToList();
            foreach (var task in allTasks)
                task.Start();

            Task.WaitAll(allTasks.ToArray());
        }

        public Task SaveItemAttribute(ItemAttribute attribute)
        {
            var itemObject = new ParseObject("CustomAttribute");
            if (!string.IsNullOrEmpty(attribute.Id))
                itemObject.ObjectId = attribute.Id;

            itemObject["ItemId"] = attribute.ItemId;
            itemObject["AttributeId"] = attribute.AttributeId;
            itemObject["Value"] = attribute.Value;
            return itemObject.SaveAsync();
        }

        

        public async Task<bool> DeleteItemAttributeByItemId(string itemId)
        {
            var rawItemAttr = await GetRawItemAttributeObjectByItemId(itemId);
            if (rawItemAttr == null)
                return false;

            var tasks = rawItemAttr.Select(attr => attr.DeleteAsync()).ToList();
            foreach (var task in tasks)
                task.Start();

            Task.WaitAll(tasks.ToArray());
            return true;
        }

        private async Task<IEnumerable<ParseObject>> GetRawItemAttributeObjectByItemId(string itemId)
        {
            var attrQuery = ParseObject.GetQuery("ItemAttribute");
            attrQuery.WhereEqualTo("ItemId", itemId);
            return await attrQuery.FindAsync();
        }
    }
}