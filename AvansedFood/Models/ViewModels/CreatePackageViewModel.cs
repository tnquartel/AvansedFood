using Domain.Models;
using System.ComponentModel.DataAnnotations;

namespace AvansedFood.Web.ViewModels
{
    public class CreatePackageViewModel
    {
        [Required(ErrorMessage = "Naam is verplicht")]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Stad is verplicht")]
        public City City { get; set; }

        [Required(ErrorMessage = "Type maaltijd is verplicht")]
        public MealType MealType { get; set; }

        [Required(ErrorMessage = "Kantine is verplicht")]
        public int CanteenId { get; set; }

        [Required(ErrorMessage = "Ophaaltijd is verplicht")]
        [DataType(DataType.DateTime)]
        public DateTime PickupTime { get; set; }

        [Required(ErrorMessage = "Verlooptijd is verplicht")]
        [DataType(DataType.DateTime)]
        public DateTime ExpirationTime { get; set; }

        [Required(ErrorMessage = "Prijs is verplicht")]
        [Range(0.01, 1000.00)]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Selecteer minimaal 1 product")]
        public List<int> SelectedProductIds { get; set; } = new();

        public List<Canteen> AvailableCanteens { get; set; } = new();
        public List<Product> AvailableProducts { get; set; } = new();
    }
}