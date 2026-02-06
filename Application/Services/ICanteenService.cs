using Domain.Models;

namespace Application.Services
{
    public interface ICanteenService
    {
        IEnumerable<Canteen> GetAllCanteens();
        Canteen? GetCanteenById(int id);
        Canteen? GetCanteenByLocation(Location location);

        bool CanOfferHotMeal(int canteenId);
    }
}