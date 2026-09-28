using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecom.Core.DTOs
{
    public record LoginDTO
    {
        public string Password { get; set; }

        public string Email { get; set; }
    }
}
