using Ecom.Core.Entites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Ecom.Core.Interfaces
{
    public interface IGenericRepositry<TEntity,TKey> where TEntity : BaseEntity<TKey>
    {
        Task<IReadOnlyList<TEntity>> GetAllAsync();
        Task<IReadOnlyList<TEntity>> GetAllAsync(params Expression<Func<TEntity, object>>[] includes);

        Task AddAsync(TEntity entity);
        Task<TEntity?> GetByIdAsync(TKey id);
        Task<TEntity?> GetByIdAsync(TKey id, params Expression<Func<TEntity, object>>[] includes);

        Task DeleteAsync(TKey id);    
        void UpdateAsync(TEntity entity);   
        Task SaveChangesAsync ();

    }
}
