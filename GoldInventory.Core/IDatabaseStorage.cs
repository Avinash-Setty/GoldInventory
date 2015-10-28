using System.Collections.Generic;
using System.Threading.Tasks;

namespace GoldInventory.Core
{
    public interface IDatabaseStorage
    {
        Task<IEnumerable<object>> GetAll<T>();

        IEnumerable<T> Query<T>(IEnumerable<Condition> conditions);

        bool Delete<T>();

        bool Save<T>(T objectToSave);

        T Create<T>(T objectToCreate);
    }
}
