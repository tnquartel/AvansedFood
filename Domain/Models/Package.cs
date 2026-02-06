using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Models
{
    public class Package
    {
        public int PackageId { get; set; }

        [Required(ErrorMessage = "Naam is verplicht")]
        [StringLength(100, ErrorMessage = "Naam mag maximaal 100 karakters zijn")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Stad is verplicht")]
        public City City { get; set; }

        [Required(ErrorMessage = "Type maaltijd is verplicht")]
        public MealType MealType { get; set; }

        [Required]
        public int CanteenId { get; set; }

        [Required(ErrorMessage = "Ophaaltijd is verplicht")]
        public DateTime PickupTime { get; set; }

        [Required(ErrorMessage = "Verlooptijd is verplicht")]
        public DateTime ExpirationTime { get; set; }

        public bool Is18Plus { get; set; }

        [Required(ErrorMessage = "Prijs is verplicht")]
        [Range(0.01, 1000.00, ErrorMessage = "Prijs moet tussen €0.01 en €1000.00 zijn")]
        [Column(TypeName = "decimal(10,2)")]
        public decimal Price { get; set; }


        public int? ReservedByStudentId { get; set; }


        public Student? ReservedByStudent { get; set; }
        public Canteen Canteen { get; set; } = null!;
        public ICollection<PackageProducts> PackageProducts { get; set; } = new List<PackageProducts>();


        public bool IsAvailable()
        {
            return ReservedByStudentId == null && ExpirationTime > DateTime.Now;
        }

        public bool IsReservedBy(int studentId)
        {
            return ReservedByStudentId == studentId;
        }

        public void UpdateIs18Plus()
        {
            Is18Plus = PackageProducts?.Any(pp => pp.Product?.ContainsAlcohol == true) ?? false;
        }
    }
}