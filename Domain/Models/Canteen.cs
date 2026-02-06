using System.ComponentModel.DataAnnotations;

namespace Domain.Models
{
    public class Canteen
    {
        public int CanteenId { get; set; }

        [Required(ErrorMessage = "Stad is verplicht")]
        public City City { get; set; }

        [Required(ErrorMessage = "Locatie is verplicht")]
        public Location Location { get; set; }

        public bool OffersHotMeals { get; set; }

        public ICollection<Package> Packages { get; set; } = new List<Package>();
        public ICollection<CanteenEmployee> Employees { get; set; } = new List<CanteenEmployee>();
    }
}