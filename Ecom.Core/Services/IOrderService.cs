using Ecom.Core.DTOs;
using Ecom.Core.Entites.Order;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecom.Core.Services
{
    public interface IOrderService
    {
        Task<OrderToReturnDTO> CreateOrderAsync(OrderDTO order,string BuyerEmail);
        Task<IReadOnlyList<OrderToReturnDTO>> GetALlOrdersForUserAsync(string BuyerEmail);
        Task<OrderToReturnDTO> GetOrderByIdAsync(int Id, string BuyerEmail);
        Task<IReadOnlyList<DeliveryMethod>> GetDeliveryMethodAsync();
    }
}
