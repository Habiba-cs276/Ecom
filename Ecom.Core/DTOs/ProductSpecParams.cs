using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecom.Core.DTOs
{
    public class ProductSpecParams
    {
        private const int MaxSize = 50;

        public int PageIndex { get; set; } = 1 ;

        private int _pageSize = 6;
        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = (value > MaxSize) ? MaxSize : value;
        }
        public int? CategoryId { get; set; }    
        public string? Sort { get; set; }
        private string? _search;
        public string? Search
        {
            get => _search;
            set => _search = value?.Trim().ToLower();
        }
    }
}
