using Domain.Models;

namespace AvansedFood.Web.GraphQL.Types
{
    public class ProductType
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool ContainsAlcohol { get; set; }
        public string? PhotoUrl { get; set; }

        public static ProductType FromDomain(Product product)
        {
            return new ProductType
            {
                Id = product.ProductId,
                Name = product.Name,
                ContainsAlcohol = product.ContainsAlcohol,
                PhotoUrl = product.PhotoUrl
            };
        }
    }
}