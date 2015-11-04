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

        public async Task<IEnumerable<ItemAttribute>> GetBareAttributes()
        {
            var allAttrs = await new AttributeHelper().GetAllAttributes();
            return allAttrs.Select(a => new ItemAttribute
            {
                AttributeName = a.Name,
                AttributeId = a.Id,
                AttributeType = a.Type
            });
        }

        public async Task<IEnumerable<ParseObject>> GetAllRawItemAttributesByIds(IEnumerable<string> ids)
        {
            var attrQuery = ParseObject.GetQuery("ItemAttribute");
            attrQuery.WhereContainedIn("Id", ids);
            var attrs = await attrQuery.FindAsync();
            if (attrs == null)
                return new List<ParseObject>();

           return attrs;
        }

        public async Task SaveAllItemAttributes(IEnumerable<ItemAttribute> attributes)
        {
            var allTasks = attributes.Select(SaveItemAttribute).ToList();
            await Task.WhenAll(allTasks);
        }

        public Task SaveItemAttribute(ItemAttribute attribute)
        {
            var itemObject = new ParseObject("ItemAttribute");
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
            await Task.WhenAll(tasks);
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