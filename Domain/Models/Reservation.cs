using System.ComponentModel.DataAnnotations;

namespace Domain.Models
{
    public class Reservation
    {
        public int ReservationId { get; set; }

        [Required]
        public int PackageId { get; set; }

        [Required]
        public int StudentId { get; set; }

        public bool IsPickedUp { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public Student Student { get; set; } = null!;
        public Package Package { get; set; } = null!;
    }
}