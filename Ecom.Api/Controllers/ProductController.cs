using AutoMapper;
using Ecom.Api.Helper;
using Ecom.Core;
using Ecom.Core.DTOs;
using Ecom.Core.Entites.Product;
using Ecom.Core.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;

namespace Ecom.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableRateLimiting("ip-limiter")]
    public class ProductController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IImageManagementService _imageManagementService;
        public ProductController(IUnitOfWork unitOfWork, IMapper mapper, IImageManagementService imageManagementService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _imageManagementService = imageManagementService;
        }
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                var model = await _unitOfWork.GetRepositry<Product, int>()
                     .GetAllAsync(x => x.Category, x => x.Photos);
                if (model == null)
                {
                    return BadRequest(new ResponseAPI<string>(400));
                }
                var Productdto = _mapper.Map<IReadOnlyList<ProductDTO>>(model);

                return Ok(new ResponseAPI<IReadOnlyList<ProductDTO>>(200, Productdto));
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseAPI<string>(400, ex.Message));
            }
        }

        [HttpGet("Get-By-ID/{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var Product = await _unitOfWork.GetRepositry<Product, int>().GetByIdAsync(id, x => x.Category, x => x.Photos);
                if (Product == null)
                {
                    return BadRequest(new ResponseAPI<string>(400));
                }
                var Productdto = _mapper.Map<ProductDTO>(Product);

                return Ok(new ResponseAPI<ProductDTO>(200, Productdto));
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseAPI<string>(400, ex.Message));
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateProductDTO productdto)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(new ResponseAPI<string>(400));

                //List<string> imagePaths = new List<string>();

                var product = _mapper.Map<Product>(productdto);

                if (productdto.Photos != null && productdto.Photos.Any())
                {
                    var imagePaths = await _imageManagementService.AddImageAsync(productdto.Photos, productdto.Name);
                    product.Photos = imagePaths.Select(path => new Photo
                    {
                        ImageName = path
                    }).ToList();
                }

                await _unitOfWork.GetRepositry<Product, int>().AddAsync(product);
                await _unitOfWork.CompleteAsync();

                return Ok(new ResponseAPI<CreateProductDTO>(200, productdto));
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseAPI<string>(400, ex.Message));
            }
        }

        [HttpPut]
        public async Task<IActionResult> Update(int id, [FromForm] UpdateProductDTO updateProductDTO)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(new ResponseAPI<string>(400));

                var product = await _unitOfWork.GetRepositry<Product, int>().GetByIdAsync(id,p=>p.Photos);

                if (product == null) return BadRequest(new ResponseAPI<string>(400));

                _mapper.Map(updateProductDTO, product);

                if (updateProductDTO.PhotoToBeDeleted != null && updateProductDTO.PhotoToBeDeleted.Any())
                {
                    foreach (var photo in updateProductDTO.PhotoToBeDeleted)
                    {
                        var photoEntity = product.Photos.FirstOrDefault(p => p.ImageName == photo);
                        if (photoEntity != null)

                        {
                            await _imageManagementService.DeleteAsync(photo);
                            product.Photos.Remove(photoEntity);
                        }
                    }

                }

                if (updateProductDTO.NewPhotos != null && updateProductDTO.NewPhotos.Any())
                {
                    var imagePaths = await _imageManagementService.AddImageAsync(updateProductDTO.NewPhotos, updateProductDTO.Name);
                    //Way 1  To Add New List 
                    //product.Photos = imagePaths.Select(path => new Photo
                    //{
                    //    ImageName = path
                    //}).ToList();

                    //Way 2

                    foreach (var photo in imagePaths)
                    {
                        product.Photos.Add(new Photo { ImageName = photo });
                    }
                }

                await _unitOfWork.CompleteAsync();

                return Ok(new ResponseAPI<UpdateProductDTO>(200, updateProductDTO));
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseAPI<string>(400, ex.Message));

            }
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var repo = _unitOfWork.GetRepositry<Product, int>();
                var product = await repo.GetByIdAsync(id,p=>p.Photos,x=>x.Category);

                if (product == null)
                    return NotFound(new ResponseAPI<string>(400));
                if (product.Photos != null && product.Photos.Any())
                {
                    foreach (var photo in product.Photos)
                    {
                        await _imageManagementService.DeleteAsync(photo.ImageName);
                    }
                }
                await repo.DeleteAsync(id);

                await _unitOfWork.CompleteAsync();

                return Ok(new ResponseAPI<Product>(200, product, $"{product.Name} Deleted Succfully"));
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseAPI<string>(400, ex.Message));
            }

        }

        // Using Extention Methods to Filter , Sort , group by category id
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] ProductSpecParams specParams)
        {
            var query = _unitOfWork.GetRepositry<Product, int>()
                .GetQueryable()
                .Include(p => p.Category)
                .Include(p => p.Photos)
                .FilterByCategoryID(specParams.CategoryId)
                .SearchByNameOrDescription(specParams.Search)
                .SortBy(specParams.Sort);

            var paginatedProducts = await query.ToPaginatedListAsync<Product>(specParams.PageIndex, specParams.PageSize);

          // mapping them cause we dont rerturn all fields in Product to front 
            var mappedItems = _mapper.Map<IReadOnlyList<ProductDTO>>(paginatedProducts.Items);

            var result = new PaginatedList<ProductDTO>(
            mappedItems,
            paginatedProducts.TotalCount,
            paginatedProducts.PageIndex,
            paginatedProducts.PageSize
            );
            return Ok(new ResponseAPI<PaginatedList<ProductDTO>>(200, result));
        }


    }
}