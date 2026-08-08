using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecom.Core.DTOs
{
    public record PhotoDTO
    {
        public string ImageName { get; set; }
        public int ProductId { get; set; }
    }
}
