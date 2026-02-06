using Domain.Models;

namespace AvansedFood.Web.GraphQL.Types
{
    public class CanteenType
    {
        public int Id { get; set; }
        public string City { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public bool OffersHotMeals { get; set; }

        public static CanteenType FromDomain(Canteen canteen)
        {
            return new CanteenType
            {
                Id = canteen.CanteenId,
                City = canteen.City.ToString(),
                Location = canteen.Location.ToString(),
                OffersHotMeals = canteen.OffersHotMeals
            };
        }
    }
}