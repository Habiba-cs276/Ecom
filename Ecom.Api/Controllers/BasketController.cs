using AutoMapper;
using Ecom.Api.Helper;
using Ecom.Core.Entites;
using Ecom.Core.Entites.Product;
using Ecom.Core.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Ecom.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BasketController : ControllerBase
    {
      
        private readonly ICustomerBasketRepositry _customerBasketRepositry;
        public BasketController( ICustomerBasketRepositry customerBasketRepositry)
        {
           
            _customerBasketRepositry = customerBasketRepositry;
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            var result = await _customerBasketRepositry.GetCustomerBasketAsync(id);
           if(result is null)
            {
                return Ok(new ResponseAPI<CustomerBasket>(200,new CustomerBasket { Id = id}));
            }
           else
            {
                return Ok(new ResponseAPI<CustomerBasket>(200, result));
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddAndUpdate(CustomerBasket customerBasket)
        {
            var _basket = await _customerBasketRepositry.UpdateBasketAsync(customerBasket);

            if (_basket is null)
            {
                return BadRequest(new ResponseAPI<CustomerBasket>(400, null, "Problem updating the basket."));
            }

            return Ok(new ResponseAPI<CustomerBasket>(200, _basket));

        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var result = await _customerBasketRepositry.DeleteBasketAsync(id);
            if (result)
            {
                return Ok(new ResponseAPI<CustomerBasket>(200,null,"Item Deleted Succefully"));
            }
            else
            {
                return BadRequest(new ResponseAPI<CustomerBasket>(400));
            }
        }


    }
}
