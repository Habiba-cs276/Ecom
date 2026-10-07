using Ecom.Core.Entites.Order;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecom.Core.DTOs
{
    public record OrderToReturnDTO
    {
        public int Id { get; set; }
        public string BuyerEmail { get; set; }
        public decimal SubTotal { get; set; }
        public decimal Total { get; set; }
        public DateTime OrderDate { get; set; }
        public ShippingAddress shippingAddress { get; set; }
        public IReadOnlyList<OrderItemDTO> orderItems { get; set; }
        public string deliveryMethod { get; set; }

        public string status { get; set; }
    }
}
