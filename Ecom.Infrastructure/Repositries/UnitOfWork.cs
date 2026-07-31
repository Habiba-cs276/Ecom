using Ecom.Core.Entites;
using Ecom.Core.Interfaces;
using Ecom.Infrastructure.Data;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecom.Infrastructure.Repositries
{
    public class UnitOfWork :IUnitOfWork
    {

        private readonly EcomDbContext _context;
        private readonly ConcurrentDictionary<Type, object> _repositores;
        public UnitOfWork(EcomDbContext context)
        {
            _context = context; 
            _repositores = new ConcurrentDictionary<Type, object>();    
        }
        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();   
        }

        public async ValueTask DisposeAsync()
        {
             await _context.DisposeAsync();   
        }

        public IGenericRepositry<TEntity, TKey> GetRepositry<TEntity, TKey>() where TEntity : BaseEntity<TKey>
        {
           var typeKey = typeof(TEntity);
            return (IGenericRepositry<TEntity, TKey>)_repositores.GetOrAdd(typeKey, value =>
            {
                return new GenericRepositry<TEntity, TKey>(_context);
            });
        }
    }
}
