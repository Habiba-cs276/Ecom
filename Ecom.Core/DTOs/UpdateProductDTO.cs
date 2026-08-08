using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecom.Core.DTOs
{
    public record UpdateProductDTO
    {
        public int Id { get; set; } 
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal NewPrice { get; set; }
        public decimal OldPrice { get; set; }
        public List<string>? PhotoToBeDeleted { get; set; }
        public IFormFileCollection? NewPhotos { get; set; }
        public int CategoryId { get; set; }
    }
}
