namespace AvansedFood.Web.ViewModels
{
    public class UserDetailViewModel
    {
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public int? StudentId { get; set; }
        public int? CanteenEmployeeId { get; set; }
        public string? StudentNumber { get; set; }
        public string? Name { get; set; }
        public DateTime? BirthDate { get; set; }
        public string? City { get; set; }
        public int NoShowCount { get; set; }
    }
}