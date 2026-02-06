using Domain.Models;
using Domain.Repositories;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class PackageRepository : IPackageRepository
    {
        private readonly AppDbContext _context;

        public PackageRepository(AppDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Package> GetAll()
            => _context.Packages.ToList();

        public Package? GetById(int id)
            => _context.Packages.Find(id);

        public void Add(Package package)
        {
            _context.Packages.Add(package);
            _context.SaveChanges();
        }

        public void Update(Package package)
        {
            var local = _context.Set<Package>()
                .Local
                .FirstOrDefault(p => p.PackageId == package.PackageId);

            if (local != null)
            {
                _context.Entry(local).State = EntityState.Detached;
            }

            _context.Packages.Update(package);
            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            var p = GetById(id);
            if (p != null)
            {
                _context.Packages.Remove(p);
                _context.SaveChanges();
            }
        }

        public IEnumerable<Package> GetAvailable()
            => _context.Packages
                .Where(p => p.ReservedByStudentId == null && p.ExpirationTime > DateTime.Now)
                .ToList();

        public IEnumerable<Package> GetAvailablePackages(City? city, MealType? mealType)
        {
            var query = _context.Packages
                .Include(p => p.Canteen)
                .Include(p => p.PackageProducts)
                    .ThenInclude(pp => pp.Product)
                .Where(p => p.ReservedByStudentId == null && p.ExpirationTime > DateTime.Now);

            if (city.HasValue)
            {
                query = query.Where(p => p.City == city.Value);
            }

            if (mealType.HasValue)
            {
                query = query.Where(p => p.MealType == mealType.Value);
            }

            return query.OrderBy(p => p.PickupTime).ToList();
        }

        public IEnumerable<Package> GetPackagesByCanteen(int canteenId)
        {
            return _context.Packages
                .Include(p => p.Canteen)
                .Where(p => p.CanteenId == canteenId)
                .OrderBy(p => p.PickupTime)
                .ToList();
        }

        public IEnumerable<Package> GetReservedPackagesByStudent(int studentId)
        {
            return _context.Packages
                .Include(p => p.Canteen)
                .Include(p => p.PackageProducts)
                    .ThenInclude(pp => pp.Product)
                .Where(p => p.ReservedByStudentId == studentId)
                .OrderBy(p => p.PickupTime)
                .ToList();
        }

        public Package? GetPackageWithDetails(int id)
        {
            return _context.Packages
                .Include(p => p.Canteen)
                .Include(p => p.PackageProducts)
                    .ThenInclude(pp => pp.Product)
                .FirstOrDefault(p => p.PackageId == id);
        }

        public bool IsAvailable(int packageId)
        {
            var package = _context.Packages.Find(packageId);
            return package != null && package.ReservedByStudentId == null;
        }

        public bool HasReservation(int packageId)
        {
            return _context.Reservations.Any(r => r.PackageId == packageId);
        }

        public void AddPackageProduct(PackageProducts packageProduct)
        {
            _context.Set<PackageProducts>().Add(packageProduct);
            _context.SaveChanges();
        }

        public void RemovePackageProduct(PackageProducts packageProduct)
        {
            _context.Set<PackageProducts>().Remove(packageProduct);
            _context.SaveChanges();
        }

        public IEnumerable<PackageProducts> GetPackageProducts(int packageId)
        {
            return _context.Set<PackageProducts>()
                .Where(pp => pp.PackageId == packageId)
                .ToList();
        }
    }
}