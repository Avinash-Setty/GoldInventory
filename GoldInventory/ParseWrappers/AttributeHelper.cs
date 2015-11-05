using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GoldInventory.Model;
using Parse;
using WebGrease.Css.Extensions;

namespace GoldInventory.ParseWrappers
{
    public class AttributeHelper
    {
        public async Task<IEnumerable<CustomAttribute>> GetAllAttributes()
        {
            var currentUser = await UserUtility.GetCurrentParseUser();
            if (currentUser == null)
                return new List<CustomAttribute>();

            var attrQuery = ParseObject.GetQuery("CustomAttribute");
            attrQuery.WhereEqualTo("CompanyId", currentUser["CompanyId"].ToString());
            var attrs = await attrQuery.FindAsync();
            if (attrs == null)
                return new List<CustomAttribute>();
            
            var filteredAttrs = new List<CustomAttribute>();
            foreach (var attr in attrs)
            {
                if (!attr.ContainsKey("CompanyId"))
                    continue;

                if (attr["CompanyId"].ToString() != currentUser["CompanyId"].ToString())
                    continue;

                filteredAttrs.Add(new CustomAttribute
                {
                    Id = attr.ObjectId,
                    CreatedAt = attr.CreatedAt,
                    UpdatedAt = attr.UpdatedAt,
                    Name = attr["Name"]?.ToString(),
                    Type = attr["Type"]?.ToString(),
                });
            }

            return filteredAttrs;
        }

        public async Task SaveCustomAttribute(CustomAttribute attribute)
        {
            var currentUser = await UserUtility.GetCurrentParseUser();
            if (currentUser == null)
                return;

            var itemObject = new ParseObject("CustomAttribute");
            if (!string.IsNullOrEmpty(attribute.Id))
                itemObject.ObjectId = attribute.Id;

            itemObject["Name"] = attribute.Name;
            itemObject["Type"] = attribute.Type;
            itemObject["CompanyId"] = currentUser["CompanyId"].ToString();
            await itemObject.SaveAsync();
        }

        public async Task<CustomAttribute> GetCustomAttributeById(string id)
        {
            var attributeObject = await GetRawCustomAttributeObjectById(id);
            return new CustomAttribute
            {
                Id = attributeObject.ObjectId,
                Name = attributeObject.Get<string>("Name"),
                Type = attributeObject.Get<string>("Type"),
                UpdatedAt = attributeObject.UpdatedAt,
                CreatedAt = attributeObject.CreatedAt
            };
        }

        private async Task<ParseObject> GetRawCustomAttributeObjectById(string id)
        {
            var currentUser = await UserUtility.GetCurrentParseUser();
            if (currentUser == null)
                return null;

            var attrQuery = ParseObject.GetQuery("CustomAttribute");
            var attributeObject = await attrQuery.GetAsync(id);
            if (currentUser["CompanyId"].ToString() != attributeObject["CompanyId"].ToString())
                return null;

            return attributeObject;
        }

        public async Task<bool> DeleteCustomAttributeById(string id)
        {
            var rawItem = await GetRawCustomAttributeObjectById(id);
            if (rawItem == null)
                return false;

            var allItemAttributes = (await new ItemAttributeHelper().GetAllRawItemAttributesByAttrIds(new List<string> {id}))?.ToList();
            if (allItemAttributes != null && allItemAttributes.Any())
            {
                var deleteTasks = allItemAttributes.Select(attr => attr.DeleteAsync()).ToList();
                await Task.WhenAll(deleteTasks.ToArray());
            }

            await rawItem.DeleteAsync();
            return true;
        }
    }
}