namespace Ecom.Core.DTOs
{
    public record OrderDTO
    {
        public int DeleviryMethodId { get; set; }
        public string BasketId { get; set; }
        public ShipAddressDTO shipAddressDTO { get; set; }  

    } 
}