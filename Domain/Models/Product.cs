using System.ComponentModel.DataAnnotations;

namespace Domain.Models
{
    public class Product
    {
        public int ProductId { get; set; }

        [Required(ErrorMessage = "Naam is verplicht")]
        [StringLength(100, ErrorMessage = "Naam mag maximaal 100 karakters zijn")]
        public string Name { get; set; } = string.Empty;

        public bool ContainsAlcohol { get; set; }

        [Url(ErrorMessage = "Ongeldige URL")]
        public string? PhotoUrl { get; set; }

        public ICollection<PackageProducts> PackageProducts { get; set; } = new List<PackageProducts>();
    }
}