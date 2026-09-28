using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecom.Core.DTOs
{
    public record RegisterDTO:LoginDTO
    {
        public string UserName {  get; set; }  
        public string DisplayName {  get; set; }    

    }
}
