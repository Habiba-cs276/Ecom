using Ecom.Core.Entites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecom.Core.Interfaces
{
    public interface ICustomerBasketRepositry
    {
        Task<CustomerBasket>GetCustomerBasketAsync(string id );    
        Task <CustomerBasket>UpdateBasketAsync( CustomerBasket basket );
        Task<bool> DeleteBasketAsync( string id );    

    }
}
