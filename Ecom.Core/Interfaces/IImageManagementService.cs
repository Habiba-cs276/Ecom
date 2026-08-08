using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecom.Core.Interfaces
{
    public interface IImageManagementService
    {
        public Task<List<string>> AddImageAsync(IFormFileCollection files, string categoryFolder);
        public Task DeleteAsync(string src);

    }
}
