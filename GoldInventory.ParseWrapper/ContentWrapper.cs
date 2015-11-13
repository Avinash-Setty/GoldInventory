using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GoldInventory.Core;
using GoldInventory.Model;
using Parse;

namespace GoldInventory.ParseWrapper
{
    public class ContentWrapper: IDatabaseStorage
    {
        public async Task<IEnumerable<object>> GetAll<T>()
        {
            var typeName = typeof (T).FullName;
            var query = ParseObject.GetQuery(typeName);
            if(query == null)
                throw new ArgumentException($"{typeName} does not exists in Parse");

            var results = await query.FindAsync();
            if (results == null)
                return null;

            switch (typeName)
            {
                case "Item":
                    return results.Select(r => new Item
                    {
                        Name = r["Name"]?.ToString(),
                        Id = r.ObjectId,
                        UpdatedAt = r.UpdatedAt,
                        CreatedAt = r.CreatedAt
                    });
                case "Company":
                    return results.Select(r => new Company
                    {
                        Name = r["Name"]?.ToString(),
                        Id = r.ObjectId,
                        Country = r["Country"]?.ToString(),
                        State = r["State"]?.ToString(),
                        City = r["City"]?.ToString(),
                        AddressLine1 = r["AddressLine1"]?.ToString(),
                        AddressLine2 = r["AddressLine2"]?.ToString(),
                        Description = r["Description"]?.ToString()
                    });
            }

            throw new ArgumentException($"{typeName} does not match any predefined objects");
        }

        public IEnumerable<T> Query<T>(IEnumerable<Condition> conditions)
        {
            throw new NotImplementedException();
        }

        public bool Delete<T>()
        {
            throw new NotImplementedException();
        }

        public bool Save<T>(T objectToSave)
        {
            throw new NotImplementedException();
        }

        public T Create<T>(T objectToCreate)
        {
            throw new NotImplementedException();
        }
    }
}
