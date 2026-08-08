using AutoMapper;
using Ecom.Core.DTOs;
using Ecom.Core.Entites.Product;

namespace Ecom.Api.Mapping
{
    public class PhotoMapping:Profile
    {
        public PhotoMapping()
        {
            CreateMap<Photo,PhotoDTO>();    
        }
    }
}
