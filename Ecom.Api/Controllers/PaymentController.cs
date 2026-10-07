using Ecom.Api.Helper;
using Ecom.Core.Entites;
using Ecom.Core.Entites.Product;
using Ecom.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Ecom.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }
        [HttpPost("Create")]
        public async Task<IActionResult>Create(string basketId,int? deliveryMethodId)
        {
            var result =  await _paymentService.CreateOrUpdatePaymentAsync(basketId,deliveryMethodId);
            return Ok(new ResponseAPI<CustomerBasket>(200,result));
        }
    }
}
