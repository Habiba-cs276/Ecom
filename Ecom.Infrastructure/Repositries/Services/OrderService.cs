using AutoMapper;
using Ecom.Core.DTOs;
using Ecom.Core.Entites.Order;
using Ecom.Core.Entites.Product;
using Ecom.Core.Interfaces;
using Ecom.Core.Services;
using Org.BouncyCastle.Crypto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecom.Infrastructure.Repositries.Services
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICustomerBasketRepositry _customerBasketRepositry;
        private readonly IMapper _mapper;
        private readonly IPaymentService _paymentService;

        public OrderService(IUnitOfWork unitOfWork, ICustomerBasketRepositry customerBasketRepositry,
            IMapper mapper, IPaymentService paymentService)
        {

            _unitOfWork = unitOfWork;
            _customerBasketRepositry = customerBasketRepositry;
            _mapper = mapper;
            _paymentService = paymentService;
        }

        public async Task<OrderToReturnDTO> CreateOrderAsync(OrderDTO orderDTO, string buyerEmail)
        {
            var BasketRedis = await _customerBasketRepositry.GetCustomerBasketAsync(orderDTO.BasketId);

            if (BasketRedis is null || BasketRedis.baseketItems is null || !BasketRedis.baseketItems.Any())
                return null;
          
            var OrderItems = new List<OrderItem>();

            foreach (var item in BasketRedis.baseketItems)
            {
                
                var product = await _unitOfWork.GetRepositry<Product, int>().GetByIdAsync(item.ProductId);

                if (product is null) continue;

                var orderItem = new OrderItem
                { 
                    ProductId = product.Id,
                    ProductName = product.Name,
                    MainImage = item.Image,
                    Price = product.NewPrice, 
                    Quantity = item.Quantity
                };

                OrderItems.Add(orderItem);
            }
            var DeliveryMethod = await _unitOfWork.GetRepositry<DeliveryMethod, int>().GetByIdAsync(orderDTO.DeleviryMethodId);
           
            if (DeliveryMethod is null)
                return null;
            
            var subTotal = OrderItems.Sum(item => item.Price * item.Quantity);

            var ShippingAddress = _mapper.Map<ShippingAddress>(orderDTO.shipAddressDTO);

            var ExistOrder = await _unitOfWork.GetRepositry<Orders, int>()
                .GetFirstOrDefaultAsync(e => e.PaymentIntentId == BasketRedis.PaymentIntentId);
            if(ExistOrder is not null)
            {
               await _unitOfWork.GetRepositry<Orders,int>().DeleteAsync(ExistOrder.Id);
               await _paymentService.CreateOrUpdatePaymentAsync(BasketRedis.Id, DeliveryMethod.Id);
            }
            var Order = new Orders { BuyerEmail = buyerEmail , deliveryMethod = DeliveryMethod, 
                orderItems = OrderItems, shippingAddress = ShippingAddress,
                SubTotal= subTotal,PaymentIntentId=BasketRedis.PaymentIntentId };

            await _unitOfWork.GetRepositry<Orders, int>().AddAsync(Order);

            var result=   await _unitOfWork.CompleteAsync();
           
            if (result <= 0) 
                return null;
            
            await _customerBasketRepositry.DeleteBasketAsync(orderDTO.BasketId);

            return _mapper.Map<OrderToReturnDTO>(Order);
        }
        public async Task<IReadOnlyList<OrderToReturnDTO>> GetALlOrdersForUserAsync(string BuyerEmail)
        {
            var orders = await _unitOfWork.GetRepositry<Orders, int>().GetAllAsync(o => o.BuyerEmail == BuyerEmail,
                inc => inc.orderItems, inc => inc.deliveryMethod);

            var OrdersAfterMapping =_mapper.Map< IReadOnlyList<OrderToReturnDTO>>(orders);
          
            return OrdersAfterMapping;
        }

        public async Task<IReadOnlyList<DeliveryMethod>> GetDeliveryMethodAsync()
        {
            var DeliverMethods = await _unitOfWork.GetRepositry<DeliveryMethod, int>().GetAllAsync();
           
            return DeliverMethods ;
        }

        public async Task<OrderToReturnDTO> GetOrderByIdAsync(int Id, string BuyerEmail)
        {
            var Order = await _unitOfWork.GetRepositry<Orders, int>().GetFirstOrDefaultAsync
                (o=>o.Id==Id&&o.BuyerEmail== BuyerEmail,
                inc=>inc.orderItems,inc=>inc.deliveryMethod);

            var OrderAfterMapping = _mapper.Map<OrderToReturnDTO>(Order);

            return OrderAfterMapping;
        }
    }
}
