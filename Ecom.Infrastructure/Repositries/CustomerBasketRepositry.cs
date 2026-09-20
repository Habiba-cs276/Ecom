using Ecom.Core.Entites;
using Ecom.Core.Interfaces;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Ecom.Infrastructure.Repositries
{
    public class CustomerBasketRepositry : ICustomerBasketRepositry
    {
        private readonly IDatabase _dataBase;
        public CustomerBasketRepositry(IConnectionMultiplexer redis)
        {
            _dataBase = redis.GetDatabase();    
        }

        public async Task<bool> DeleteBasketAsync(string id)
        {
           return await _dataBase.KeyDeleteAsync(id);
        }

        public async Task<CustomerBasket> GetCustomerBasketAsync(string id)
        {
            var result = await _dataBase.StringGetAsync(id);
            if(!string.IsNullOrEmpty(result))
            {
                return JsonSerializer.Deserialize<CustomerBasket>(result);
            }
            return null;
           
        }

        public async Task<CustomerBasket> UpdateBasketAsync(CustomerBasket basket)
        {
            var result = await _dataBase.StringSetAsync(basket.Id, JsonSerializer.Serialize(basket),TimeSpan.FromDays(3));
            if (result)
            {
                return await GetCustomerBasketAsync(basket.Id);
            }
            return null;
        }
    }
}
