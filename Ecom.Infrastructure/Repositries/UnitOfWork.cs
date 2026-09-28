using Ecom.Core.Entites;
using Ecom.Core.Interfaces;
using Ecom.Core.Services;
using Ecom.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
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

        public IAuthRepositry Auth { get; }
        public UnitOfWork(EcomDbContext context,UserManager<ApplicationUser> userManager,
            IEmailService emailService,SignInManager<ApplicationUser> signInManager,
            IAuthRepositry authRepositry)
        {
            _context = context; 
            _repositores = new ConcurrentDictionary<Type, object>();
            //_userManager = userManager;
            //_signInManager = signInManager;
            Auth = authRepositry;
            //_emailService = emailService;
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
