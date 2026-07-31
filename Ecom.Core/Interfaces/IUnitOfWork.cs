using Ecom.Core.Entites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecom.Core.Interfaces
{
    public interface IUnitOfWork: IAsyncDisposable
    {
        IGenericRepositry<TEntity,TKey> GetRepositry<TEntity,TKey>() where TEntity: BaseEntity<TKey>;
        Task<int> CompleteAsync();  
    }
}
