using Domain.Models;

namespace AvansedFood.Web.ViewModels
{
    public class PackageViewModel
    {
        public int PackageId { get; set; }
        public string Name { get; set; } = string.Empty;
        public City City { get; set; }
        public MealType MealType { get; set; }
        public string CanteenName { get; set; } = string.Empty;
        public Domain.Models.Location CanteenLocation { get; set; }
        public DateTime PickupTime { get; set; }
        public DateTime ExpirationTime { get; set; }
        public decimal Price { get; set; }
        public bool Is18Plus { get; set; }
        public bool IsReserved { get; set; }
        public List<ProductViewModel> Products { get; set; } = new();
    }

    public class ProductViewModel
    {
        public int ProductId { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool ContainsAlcohol { get; set; }
        public string? PhotoUrl { get; set; }
    }
}