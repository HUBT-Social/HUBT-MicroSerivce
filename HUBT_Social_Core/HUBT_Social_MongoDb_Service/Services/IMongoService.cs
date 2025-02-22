using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace HUBT_Social_MongoDb_Service.Services
{
    public interface IMongoService<Collection>
        where Collection : class
    {
        Task<Collection?> GetById(string id);
        Task<bool> Create(Collection collection);
        Task<bool> Delete(Collection collection);
        Task<bool> Update(Collection collection);
        Task<IEnumerable<Collection>> GetAll(int? limit = null);
        Task<IEnumerable<Collection>> Find(Expression<Func<Collection, bool>> predicate);
        Task<bool> Exists(string id);
        Task<long> Count();
    }
}
