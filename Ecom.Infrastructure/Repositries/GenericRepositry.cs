 using Ecom.Core.Entites;
using Ecom.Core.Interfaces;
using Ecom.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Ecom.Infrastructure.Repositries
{
    public class GenericRepositry<TEntity, TKey> : IGenericRepositry<TEntity, TKey> where TEntity : BaseEntity<TKey>
    {
        private readonly EcomDbContext _context;
        private readonly DbSet<TEntity> _set;
        public GenericRepositry(EcomDbContext context) 
        {
            _context = context; 
            _set = context.Set<TEntity>();
        }   
        public async Task AddAsync(TEntity entity)
        {
           await _set.AddAsync(entity); 
        }

        public async Task DeleteAsync(TKey id)
        {
            TEntity? entity = await GetByIdAsync(id);

            if(entity != null) 
             _set.Remove(entity);
        }

        public async Task<IReadOnlyList<TEntity>> GetAllAsync()
            => await _set.AsNoTracking().ToListAsync(); 
        public async Task<IReadOnlyList<TEntity>> GetAllAsync(params Expression<Func<TEntity, object>>[] includes)
        {
            IQueryable<TEntity>? query =_set.AsQueryable();
            foreach(var include in includes)
            {
                query=query.Include(include);
            }
            return await query.ToListAsync();   

        }

        public async Task<TEntity?> GetByIdAsync(TKey id)
        {
         return  await _set.FindAsync(id);
        }

        public Task<TEntity?> GetByIdAsync(TKey id, params Expression<Func<TEntity, object>>[] includes)
        {
            IQueryable<TEntity>? query = _set.AsQueryable();
            foreach (var include in includes)
            {
                query = query.Include(include);
            }
            return query.FirstOrDefaultAsync(i => i.Id!.Equals(id));
          }

        public async Task SaveChangesAsync()
        {
          await _context.SaveChangesAsync();
        }

        public void UpdateAsync(TEntity entity)
        {
            _context.Update(entity);
        }
    }
}
