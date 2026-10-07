using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecom.Core.Entites
{
    public  class CustomerBasket
    {
        public CustomerBasket()
        { 
        }
        public CustomerBasket(string id)
        {
            this.Id = id;   
        }
        public string Id { get; set; } 
        public string PaymentIntentId { get; set; }
        public string ClientSecret { get; set; }

        public List<BasketItem> baseketItems { get; set; } = new List<BasketItem>();
    }
}
