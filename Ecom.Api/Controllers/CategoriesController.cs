using AutoMapper;
using Ecom.Api.Helper;
using Ecom.Api.Mapping;
using Ecom.Core.DTOs;
using Ecom.Core.Entites.Product;
using Ecom.Core.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Ecom.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;  
        public CategoriesController(IUnitOfWork unitOfWork, IMapper mapper) 
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper; 
        }
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                var model = await _unitOfWork.GetRepositry<Category, int>().GetAllAsync();
                if (model == null)
                {
                    return BadRequest(new ResponseAPI<string>(400));
                }
                return Ok(new ResponseAPI<IReadOnlyList<Category>>(200,model));
            }
            catch (Exception ex) 
            {
                return BadRequest(ex.Message);  
            }
        }

        [HttpGet("id")]
        public async Task<IActionResult> GetById(int id) 
        {
            try
            {
                var model = await _unitOfWork.GetRepositry<Category, int>().GetByIdAsync(id);
                if (model == null)
                {
                    return BadRequest(new ResponseAPI<string>(400,$"Category with ID{id} Not Found"));
                }
                return Ok(new ResponseAPI<Category>(200,model));
            }
            catch (Exception ex) 
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost]
        public async Task<IActionResult> Create(CategoryDTO categorydto)
        {
            try
            {
                Category category =_mapper.Map<Category>(categorydto);  

                await _unitOfWork.GetRepositry<Category, int>().AddAsync(category);
                await _unitOfWork.CompleteAsync();    
                return Ok(new ResponseAPI<CategoryDTO>(200,categorydto));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _unitOfWork.GetRepositry<Category, int>().DeleteAsync(id);
                await _unitOfWork.CompleteAsync();
                return Ok(new ResponseAPI<string>(200, "Item has been Deleted succfully"));
            }
            catch (Exception ex) 
            {
                return BadRequest(ex.Message);
             }
        }
        [HttpPut]
        public async Task<IActionResult> Update(CategoryDTO categorydto)
        {
            try
            {
                Category category =_mapper.Map<Category>(categorydto);  

                 _unitOfWork.GetRepositry<Category, int>().UpdateAsync(category);
                 await _unitOfWork.CompleteAsync();
                return Ok(new ResponseAPI<string>(200, "Item has been Updated succfully"));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
