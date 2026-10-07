using Ecom.Core.Entites;
using Ecom.Core.Entites.Order;
using Ecom.Core.Interfaces;
using Ecom.Core.Services;
using Microsoft.Extensions.Configuration;
using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Product = Ecom.Core.Entites.Product.Product;
namespace Ecom.Infrastructure.Repositries.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IConfiguration _configuration; 
        private readonly ICustomerBasketRepositry _customerBasketRepositry;
        private readonly IUnitOfWork _unitOfWork;
        public PaymentService(ICustomerBasketRepositry customerBasketRepositry,
            IConfiguration configuration, IUnitOfWork unitOfWork)
        {
            _customerBasketRepositry = customerBasketRepositry;
            _configuration = configuration;
            _unitOfWork = unitOfWork;
        }

        public async Task<CustomerBasket> CreateOrUpdatePaymentAsync(string basketId,int? deliveryMethodId)
        {
           var Basket = await _customerBasketRepositry.GetCustomerBasketAsync(basketId);
           
            StripeConfiguration.ApiKey =_configuration["StrpieSetting:SecretKey"];
            decimal ShippingPrice = 0m;
          
            if (deliveryMethodId.HasValue)
            {
                var delivery = await _unitOfWork.GetRepositry<DeliveryMethod,int>().GetByIdAsync(deliveryMethodId.Value);
                ShippingPrice = delivery.Price;
            }
            foreach(var item in Basket.baseketItems)
            {
                var Product = await _unitOfWork.GetRepositry<Product, int>().GetByIdAsync(item.ProductId);
                item.Price = Product.NewPrice; 
            }
            PaymentIntentService paymentIntentService = new PaymentIntentService();
            PaymentIntent paymentIntent;

            if (string.IsNullOrEmpty(Basket.PaymentIntentId))
            {
                var option = new PaymentIntentCreateOptions
                {
                    Amount =(long) Basket.baseketItems.Sum(p => p.Quantity * p.Price) 
                    + (long)ShippingPrice,
                    Currency="USD",
                    AutomaticPaymentMethods = new PaymentIntentAutomaticPaymentMethodsOptions
                    {
                        Enabled = true,
                    }
                };
                paymentIntent = await paymentIntentService.CreateAsync(option);
               
                Basket.PaymentIntentId= paymentIntent.Id;
                Basket.ClientSecret= paymentIntent.ClientSecret;
            }
            else
            {
                var option = new PaymentIntentUpdateOptions
                {
                    Amount = (long)Basket.baseketItems.Sum(p => p.Quantity * p.Price)
                    + (long)ShippingPrice

                };
                await paymentIntentService.UpdateAsync(Basket.PaymentIntentId, option); 
            }
            await _customerBasketRepositry.UpdateBasketAsync(Basket);
           
            return Basket;
        }
    }
}
