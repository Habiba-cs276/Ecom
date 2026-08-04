using AutoMapper;
using Ecom.Core.DTOs;
using Ecom.Core.Entites.Product;

namespace Ecom.Api.Mapping
{
    public class CategoryMapping: Profile
    {
        public CategoryMapping()
        {
            CreateMap<CategoryDTO,Category>().ReverseMap(); 
        }
    }
}
