using System.Linq;
using Ecom.Core.Entites.Product;

namespace Ecom.Api.Helper
{
    public static class ProductQueryExtensions
    {
        // Joining by Category ID
        public static IQueryable<Product> FilterByCategoryID(this IQueryable<Product>query, int? Categoryid)
        {
            if (!Categoryid.HasValue)
            {
                return query;   
            }
            return query.Where(p=>p.CategoryId == Categoryid.Value);
        }
        // Making Smart SEarch
        public static IQueryable<Product> SearchByNameOrDescription(this  IQueryable<Product>query, string? search)
        {
            if (string.IsNullOrWhiteSpace(search))
                return query;

            var term = search.Trim().ToLower();

            return query.Where(p=>p.Name.ToLower().Contains (term)||
                                  p.Description.ToLower().Contains(term));
        }
        public static IQueryable<Product> SortBy(this IQueryable<Product> query, string? sort)
        {
            if (string.IsNullOrWhiteSpace(sort))
                return query.OrderBy(p => p.Name);

            return sort.ToLower() switch
            {
                "priceasc" => query.OrderBy(p => p.NewPrice),
                "pricedesc" => query.OrderByDescending(p => p.NewPrice),
                "name" => query.OrderBy(p => p.Name),
                _ => query.OrderBy(p => p.Id)
            };
        }
        }
}
