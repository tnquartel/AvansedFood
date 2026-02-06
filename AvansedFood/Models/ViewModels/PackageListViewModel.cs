using Domain.Models;

namespace AvansedFood.Web.ViewModels
{
    public class PackageListViewModel
    {
        public List<PackageViewModel> Packages { get; set; } = new();

        public City? SelectedCity { get; set; }
        public MealType? SelectedMealType { get; set; }

        public List<City> AvailableCities { get; set; } = new()
        {
            City.Breda,
            City.DenBosch,
            City.Tilburg
        };

        public List<MealType> AvailableMealTypes { get; set; } = new()
        {
            MealType.Brood,
            MealType.WarmeAvondmaaltijd,
            MealType.Drank
        };
    }
}