using Domain.Models;

namespace AvansedFood.Web.GraphQL.Types
{
    public class PackageType
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string MealType { get; set; } = string.Empty;
        public DateTime PickupTime { get; set; }
        public DateTime ExpirationTime { get; set; }
        public decimal Price { get; set; }
        public bool Is18Plus { get; set; }
        public bool IsReserved { get; set; }

        public CanteenType? Canteen { get; set; }
        public List<ProductType> Products { get; set; } = new();

        public static PackageType FromDomain(Package package)
        {
            return new PackageType
            {
                Id = package.PackageId,
                Name = package.Name,
                City = package.City.ToString(),
                MealType = package.MealType.ToString(),
                PickupTime = package.PickupTime,
                ExpirationTime = package.ExpirationTime,
                Price = package.Price,
                Is18Plus = package.Is18Plus,
                IsReserved = package.ReservedByStudentId != null,
                Canteen = package.Canteen != null ? CanteenType.FromDomain(package.Canteen) : null,
                Products = package.PackageProducts
                    .Select(pp => ProductType.FromDomain(pp.Product))
                    .ToList()
            };
        }
    }
}