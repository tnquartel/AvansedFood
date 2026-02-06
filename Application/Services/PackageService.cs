using Domain.Models;
using Domain.Repositories;

namespace Application.Services
{
    public class PackageService : IPackageService
    {
        private readonly IPackageRepository _packageRepository;
        private readonly ICanteenRepository _canteenRepository;
        private readonly IStudentRepository _studentRepository;
        private readonly IProductRepository _productRepository;
        private readonly IReservationRepository _reservationRepository;


        public PackageService(
            IPackageRepository packageRepository,
            ICanteenRepository canteenRepository,
            IStudentRepository studentRepository,
            IProductRepository productRepository,
            IReservationRepository reservationRepository)
        {
            _packageRepository = packageRepository;
            _canteenRepository = canteenRepository;
            _studentRepository = studentRepository;
            _productRepository = productRepository;
            _reservationRepository = reservationRepository;
        }

        public IEnumerable<Package> GetAllPackages()
        {
            return _packageRepository.GetAll();
        }

        public IEnumerable<Package> GetPackagesByCanteen(int canteenId)
        {
            return _packageRepository.GetPackagesByCanteen(canteenId);
        }

        public IEnumerable<Package> GetAvailablePackages()
        {
            return _packageRepository.GetAvailablePackages(null, null);
        }

        public IEnumerable<Package> GetAvailablePackages(City? city, MealType? mealType)
        {
            var packages = _packageRepository.GetAvailablePackages(city, mealType);

            if (city.HasValue)
            {
                packages = packages.Where(p => p.City == city.Value);
            }

            if (mealType.HasValue)
            {
                packages = packages.Where(p => p.MealType == mealType.Value);
            }

            return packages;
        }

        public Package? GetPackageDetails(int packageId)
        {
            return _packageRepository.GetPackageWithDetails(packageId);
        }

        public (bool Success, string Message) CreatePackage(Package package, int employeeId, List<int> productIds)
        {
            if (package.PickupTime > DateTime.Now.AddDays(2))
            {
                return (false, "Je mag alleen pakketten aanbieden tot maximaal 2 dagen vooruit.");
            }

            if (package.PickupTime <= DateTime.Now)
            {
                return (false, "Ophaaltijd moet in de toekomst liggen.");
            }

            if (package.ExpirationTime <= package.PickupTime)
            {
                return (false, "Verlooptijd moet na de ophaaltijd liggen.");
            }

            var canteen = _canteenRepository.GetById(package.CanteenId);
            if (canteen == null)
            {
                return (false, "Kantine niet gevonden.");
            }

            if (package.MealType == MealType.WarmeAvondmaaltijd && !canteen.OffersHotMeals)
            {
                return (false, "Deze kantine biedt geen warme avondmaaltijden aan.");
            }

            package.PackageProducts = new List<PackageProducts>();
            foreach (var productId in productIds)
            {
                var product = _productRepository.GetById(productId);
                if (product != null)
                {
                    package.PackageProducts.Add(new PackageProducts
                    {
                        PackageId = package.PackageId,
                        ProductId = productId,
                        Product = product
                    });
                }
            }

            package.UpdateIs18Plus();

            _packageRepository.Add(package);
            return (true, "Pakket succesvol aangemaakt.");
        }

        public (bool Success, string Message) UpdatePackage(Package package, List<int> productIds)
        {
            var existingPackage = _packageRepository.GetPackageWithDetails(package.PackageId);

            if (existingPackage == null)
            {
                return (false, "Pakket niet gevonden");
            }

            if (existingPackage.ReservedByStudentId != null)
            {
                return (false, "Een pakket wat gereserveerd is kan niet geupdate worden");
            }

            existingPackage.Name = package.Name;
            existingPackage.City = package.City;
            existingPackage.MealType = package.MealType;
            existingPackage.CanteenId = package.CanteenId;
            existingPackage.PickupTime = package.PickupTime;
            existingPackage.ExpirationTime = package.ExpirationTime;
            existingPackage.Price = package.Price;

            var pickupDate = existingPackage.PickupTime.Date;
            var today = DateTime.Now.Date;
            var maxDate = today.AddDays(2);

            if (pickupDate > maxDate)
            {
                return (false, "Een pakket kan maximaal 2 dagen vantevoren aangeboden worden");
            }

            if (existingPackage.MealType == MealType.WarmeAvondmaaltijd)
            {
                var canteen = _canteenRepository.GetById(existingPackage.CanteenId);
                if (canteen != null && !canteen.OffersHotMeals)
                {
                    return (false, "Deze kantine biedt geen warme maaltijden aan");
                }
            }

            var currentProducts = _packageRepository.GetPackageProducts(existingPackage.PackageId);
            foreach (var pp in currentProducts)
            {
                _packageRepository.RemovePackageProduct(pp);
            }

            foreach (var productId in productIds)
            {
                var packageProduct = new PackageProducts
                {
                    PackageId = existingPackage.PackageId,
                    ProductId = productId
                };
                _packageRepository.AddPackageProduct(packageProduct);
            }

            var updatedPackage = _packageRepository.GetPackageWithDetails(existingPackage.PackageId);
            if (updatedPackage != null)
            {
                updatedPackage.UpdateIs18Plus();
                _packageRepository.Update(updatedPackage);
            }

            return (true, "Pakket succesvol geupdate");
        }

        public (bool Success, string Message) DeletePackage(int packageId)
        {
            var package = _packageRepository.GetById(packageId);

            if (package == null)
            {
                return (false, "Pakket niet gevonden");
            }

            if (package.ReservedByStudentId != null)
            {
                return (false, "Een gereserveerd pakket kan niet worden verwijderd");
            }

            _packageRepository.Delete(packageId);
            return (true, "Pakket succesvol verwijderd");
        }

        public (bool Success, string Message) ReservePackage(int packageId, int studentId)
        {
            var package = _packageRepository.GetPackageWithDetails(packageId);
            if (package == null)
            {
                return (false, "Pakket niet gevonden");
            }

            var student = _studentRepository.GetById(studentId);
            if (student == null)
            {
                return (false, "Student niet gevonden");
            }

            var isAvailable = IsPackageAvailable(packageId);

            if (!isAvailable)
            {
                return (false, "Dit pakket is al gereserveerd");
            }

            var pickupDate = package.PickupTime.Date;

            var hasReservationOnDate = _studentRepository.HasReservationOnDate(studentId, pickupDate);

            if (hasReservationOnDate)
            {
                return (false, "Er kan maar een pakket per afhaaldatum gereserveerd worden");
            }

            if (package.Is18Plus && !student.IsAdult(package.PickupTime))
            {
                return (false, "Je moet 18 jaar of ouder zijn om dit pakket te bestellen");
            }

            if (student.NoShowCount > 2)
            {
                return (false, "Je kan dit pakket niet bestellen vanwege te veel no-shows");
            }

            var reservation = new Reservation
            {
                PackageId = packageId,
                StudentId = studentId,
                IsPickedUp = false,
                CreatedAt = DateTime.Now
            };

            package.ReservedByStudentId = studentId;

            _reservationRepository.Add(reservation);

            _packageRepository.Update(package);

            return (true, "Pakket succesvol gereserveerd");
        }

        public IEnumerable<Package> GetReservedPackagesByStudent(int studentId)
        {
            return _packageRepository.GetReservedPackagesByStudent(studentId);
        }

        public bool IsPackageAvailable(int packageId)
        {
            return _packageRepository.IsAvailable(packageId);
        }
    }
}