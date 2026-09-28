namespace Ecom.Core.Entites.Order
{
    public class DeliveryMethod:BaseEntity<int>
    {
        public DeliveryMethod()
        {
        }

        public DeliveryMethod(string name, decimal price, string description, string delivaryTime)
        {
            Name = name;
            Price = price;
            Description = description;
            DelivaryTime = delivaryTime;
        }

        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
        public string DelivaryTime { get; set; }    

        
    }
}