using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecom.Core.DTOs
{
    public record ResetPasswordDTO:LoginDTO
    {
        public string token {  get; set; }  
    }
}
