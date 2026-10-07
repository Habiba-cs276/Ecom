using AutoMapper;
using Ecom.Core.DTOs;
using Ecom.Core.Entites;
using Ecom.Core.Entites.Order;

namespace Ecom.Api.Mapping
{
    public class OrderMapping : Profile
    {
        public OrderMapping()
        {
            CreateMap<ShipAddressDTO, ShippingAddress>();

            CreateMap<Orders, OrderToReturnDTO>()
                .ForMember(dest => dest.deliveryMethod, opt => opt.MapFrom(src => src.deliveryMethod.Name))
                .ForMember(dest => dest.status, opt => opt.MapFrom(src => src.status.ToString()))
                .ForMember(dest=>dest.Total,opt=>opt.MapFrom(src=>src.GetTotal()));

            CreateMap<OrderItem, OrderItemDTO>();
          
            CreateMap<ShipAddressDTO, Address>().ReverseMap();
            //CreateMap<>

        }
    }
}
