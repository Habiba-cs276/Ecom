using Ecom.Api.Helper;
using Ecom.Core.DTOs;
using Ecom.Core.Entites.Order;
using Ecom.Core.Entites.Product;
using Ecom.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Ecom.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }
        [HttpPost("Create-Order")]
        public async Task<IActionResult> Create(OrderDTO orderDTO)
        {
            var UserEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            
            var orderToBeCreated = await _orderService.CreateOrderAsync(orderDTO, UserEmail);
         
            if (orderToBeCreated is null) return BadRequest(new ResponseAPI<OrderToReturnDTO>(404,null,"Problem creating order"));

            return Ok(new ResponseAPI<OrderToReturnDTO>(200, orderToBeCreated,"Order has been created succufully"));

        }
        [HttpGet("Get-Orders-By-User")]
        public async Task<IActionResult> GetOrdersByUser()
        {
            var UserEmail = User.FindFirst(ClaimTypes.Email)?.Value;

            var orders = await _orderService.GetALlOrdersForUserAsync(UserEmail);

            return Ok(new ResponseAPI<IReadOnlyList<OrderToReturnDTO>>(200, orders));

        }

        [HttpGet("Get-Order-By-Id/{id}")]
        public async Task<IActionResult> GetOrder(int id)
        {
            var UserEmail = User.FindFirst(ClaimTypes.Email)?.Value;

            var order = await _orderService.GetOrderByIdAsync(id, UserEmail);
           
            return Ok(new ResponseAPI<OrderToReturnDTO>(200, order));

        }

        [HttpGet("Get-Delivery-Methods")]
        [AllowAnonymous]
        public async Task<IActionResult> GetDeliveryMethods()
        {
            var DeliveryMethods = await _orderService.GetDeliveryMethodAsync();

            return Ok(new ResponseAPI<IReadOnlyList<DeliveryMethod>>(200, DeliveryMethods));

        }

    }
}
