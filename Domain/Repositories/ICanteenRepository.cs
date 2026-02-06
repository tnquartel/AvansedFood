using Domain.Models;

namespace Domain.Repositories
{
    public interface ICanteenRepository
    {
        IEnumerable<Canteen> GetAll();
        Canteen? GetById(int id);
        IEnumerable<Canteen> GetByCity(City city);
        Canteen? GetByLocation(Location location);
        void Add(Canteen canteen);
        void Update(Canteen canteen);
    }
}