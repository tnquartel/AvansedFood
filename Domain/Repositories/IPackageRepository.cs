using Domain.Models;

namespace Domain.Repositories
{
    public interface IPackageRepository
    {
        IEnumerable<Package> GetAll();
        Package? GetById(int id);
        void Add(Package package);
        void Update(Package package);
        void Delete(int id);

        IEnumerable<Package> GetAvailable();
        IEnumerable<Package> GetAvailablePackages(City? city, MealType? mealType);
        IEnumerable<Package> GetPackagesByCanteen(int canteenId);
        IEnumerable<Package> GetReservedPackagesByStudent(int studentId);
        Package? GetPackageWithDetails(int id);

        bool IsAvailable(int packageId);
        bool HasReservation(int packageId);

        void AddPackageProduct(PackageProducts packageProduct);
        void RemovePackageProduct(PackageProducts packageProduct);
        IEnumerable<PackageProducts> GetPackageProducts(int packageId);
    }
}