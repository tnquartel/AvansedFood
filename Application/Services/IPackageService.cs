using Domain.Models;

namespace Application.Services
{
    public interface IPackageService
    {

        IEnumerable<Package> GetAllPackages();
        IEnumerable<Package> GetPackagesByCanteen(int canteenId);

        IEnumerable<Package> GetAvailablePackages();
        IEnumerable<Package> GetAvailablePackages(City? city, MealType? mealType);
        Package? GetPackageDetails(int packageId);

        (bool Success, string Message) CreatePackage(Package package, int employeeId, List<int> productIds);
        (bool Success, string Message) UpdatePackage(Package package, List<int> productIds);
        (bool Success, string Message) DeletePackage(int packageId);

        (bool Success, string Message) ReservePackage(int packageId, int studentId);
        IEnumerable<Package> GetReservedPackagesByStudent(int studentId);

        bool IsPackageAvailable(int packageId);
    }
}