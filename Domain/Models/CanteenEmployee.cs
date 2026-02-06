using System.ComponentModel.DataAnnotations;

namespace Domain.Models
{
    public class CanteenEmployee
    {
        public int CanteenEmployeeId { get; set; }

        [Required(ErrorMessage = "Personeelsnummer is verplicht")]
        public string PersonnelNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Naam is verplicht")]
        [StringLength(100, ErrorMessage = "Naam mag maximaal 100 karakters zijn")]
        public string Name { get; set; } = string.Empty;

        [Required]
        public int CanteenId { get; set; }

        public Canteen Canteen { get; set; } = null!;
    }
}