using System.ComponentModel.DataAnnotations.Schema;

namespace Ecom.Core.Entites
{
    public class Address:BaseEntity<int>
    {
        public string FirstName { get; set; }

        public string LastName {  get; set; }

        public string City {  get; set; }

        public string ZipCode {  get; set; }

        public string Street { get; set; }  
        public string State { get; set; }

        public string ApplicationUserId { get; set; }

        [ForeignKey(nameof(ApplicationUserId))]
        public virtual ApplicationUser ApplicationUser { get; set; }
         
    }
}