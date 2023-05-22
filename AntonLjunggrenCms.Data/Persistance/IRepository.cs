using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntonLjunggrenCms.Data.Persistance
{
    public interface IRepository<TEntity, TKey> where TEntity : class
    {
        public Task<TEntity> GetAsync(TKey id);
        public Task<IEnumerable<TEntity>> GetAllAsync();
        public Task<bool> DeleteAsync(TKey id);
        public Task<TEntity> AddAsync(TEntity entity);
        public Task<TEntity> UpdateAsync(TEntity newEntity);
    }
}
