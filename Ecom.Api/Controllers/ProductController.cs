using Ecom.Core.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Ecom.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        ProductController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;   
        }
        [HttpGet]
        public Task<IActionResult> Get()
        {

        }
    }

}
