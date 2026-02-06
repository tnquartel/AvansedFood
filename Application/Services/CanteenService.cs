using Domain.Models;
using Domain.Repositories;

namespace Application.Services
{
    public class CanteenService : ICanteenService
    {
        private readonly ICanteenRepository _canteenRepository;

        public CanteenService(ICanteenRepository canteenRepository)
        {
            _canteenRepository = canteenRepository;
        }

        public IEnumerable<Canteen> GetAllCanteens()
        {
            return _canteenRepository.GetAll();
        }

        public Canteen? GetCanteenById(int id)
        {
            return _canteenRepository.GetById(id);
        }

        public Canteen? GetCanteenByLocation(Location location)
        {
            return _canteenRepository.GetByLocation(location);
        }

        public bool CanOfferHotMeal(int canteenId)
        {
            var canteen = _canteenRepository.GetById(canteenId);
            return canteen?.OffersHotMeals ?? false;
        }
    }
}