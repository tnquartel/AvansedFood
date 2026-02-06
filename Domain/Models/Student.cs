using System.ComponentModel.DataAnnotations;

namespace Domain.Models
{
    public class Student
    {
        public int StudentId { get; set; }

        [Required(ErrorMessage = "Studentnummer is verplicht")]
        public string StudentNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Naam is verplicht")]
        [StringLength(100, ErrorMessage = "Naam mag maximaal 100 karakters zijn")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Geboortedatum is verplicht")]
        [DataType(DataType.Date)]
        public DateTime BirthDate { get; set; }

        [Required(ErrorMessage = "E-mailadres is verplicht")]
        [EmailAddress(ErrorMessage = "Ongeldig e-mailadres")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Telefoonnummer is verplicht")]
        [Phone(ErrorMessage = "Ongeldig telefoonnummer")]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Studiestad is verplicht")]
        public City StudyCity { get; set; }

        public int NoShowCount { get; set; } = 0;

        public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();

        public int GetAge()
        {
            var today = DateTime.Today;
            var age = today.Year - BirthDate.Year;
            if (BirthDate.Date > today.AddYears(-age)) age--;
            return age;
        }

        public bool IsAdult(DateTime onDate)
        {
            var age = onDate.Year - BirthDate.Year;
            if (BirthDate.Date > onDate.AddYears(-age)) age--;
            return age >= 18;
        }

        public bool CanReserve()
        {
            return NoShowCount <= 2;
        }
    }
}
