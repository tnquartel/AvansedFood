using Application.Services;
using Domain.Models;
using Domain.Repositories;
using FluentAssertions;
using Moq;
using Xunit;

namespace AvansedFood.Tests.Services
{
    public class PackageServiceTests
    {
        private readonly Mock<IPackageRepository> _packageRepositoryMock;
        private readonly Mock<IStudentRepository> _studentRepositoryMock;
        private readonly Mock<ICanteenRepository> _canteenRepositoryMock;
        private readonly Mock<IProductRepository> _productRepositoryMock;
        private readonly Mock<IReservationRepository> _reservationRepositoryMock;
        private readonly PackageService _packageService;

        public PackageServiceTests()
        {
            _packageRepositoryMock = new Mock<IPackageRepository>();
            _studentRepositoryMock = new Mock<IStudentRepository>();
            _canteenRepositoryMock = new Mock<ICanteenRepository>();
            _productRepositoryMock = new Mock<IProductRepository>();
            _reservationRepositoryMock = new Mock<IReservationRepository>();

            _packageService = new PackageService(
                _packageRepositoryMock.Object,
                _canteenRepositoryMock.Object,
                _studentRepositoryMock.Object,
                _productRepositoryMock.Object,
                _reservationRepositoryMock.Object
            );
        }

        [Fact]
        public void ReservePackage_StudentAlreadyHasReservationOnSameDay_ShouldReturnError()
        {
            // Arrange
            var packageId = 1;
            var studentId = 1;
            var pickupDate = DateTime.Now.Date;

            var package = new Package
            {
                PackageId = packageId,
                Name = "Test Package",
                PickupTime = pickupDate.AddHours(12),
                ExpirationTime = pickupDate.AddHours(14),
                Is18Plus = false,
                City = City.Breda,
                MealType = MealType.Brood,
                Price = 5.00m,
                Canteen = new Canteen { CanteenId = 1, City = City.Breda, Location = Location.LA }
            };

            var student = new Student
            {
                StudentId = studentId,
                Name = "Test Student",
                Email = "test@avans.nl",
                BirthDate = new DateTime(2000, 1, 1),
                StudentNumber = "123456",
                StudyCity = City.Breda
            };

            _packageRepositoryMock
                .Setup(x => x.GetPackageWithDetails(packageId))
                .Returns(package);

            _studentRepositoryMock
                .Setup(x => x.GetById(studentId))
                .Returns(student);

            _packageRepositoryMock
                .Setup(x => x.IsAvailable(packageId))
                .Returns(true);

            _studentRepositoryMock
                .Setup(x => x.HasReservationOnDate(studentId, pickupDate))
                .Returns(true);

            // Act
            var result = _packageService.ReservePackage(packageId, studentId);

            // Assert
            result.Success.Should().BeFalse();
            result.Message.Should().Contain("een pakket per afhaaldatum");
        }

        [Fact]
        public void ReservePackage_ValidReservation_ShouldSucceed()
        {
            // Arrange
            var packageId = 1;
            var studentId = 1;
            var pickupDate = DateTime.Now.Date;

            var package = new Package
            {
                PackageId = packageId,
                Name = "Test Package",
                PickupTime = pickupDate.AddHours(12),
                ExpirationTime = pickupDate.AddHours(14),
                Is18Plus = false,
                City = City.Breda,
                MealType = MealType.Brood,
                Price = 5.00m,
                PackageProducts = new List<PackageProducts>(),
                Canteen = new Canteen { CanteenId = 1, City = City.Breda, Location = Location.LA }
            };

            var student = new Student
            {
                StudentId = studentId,
                Name = "Test Student",
                Email = "test@avans.nl",
                BirthDate = new DateTime(2000, 1, 1),
                StudentNumber = "123456",
                StudyCity = City.Breda,
                NoShowCount = 0
            };

            _packageRepositoryMock.Setup(x => x.GetPackageWithDetails(packageId)).Returns(package);
            _studentRepositoryMock.Setup(x => x.GetById(studentId)).Returns(student);
            _packageRepositoryMock.Setup(x => x.IsAvailable(packageId)).Returns(true);
            _studentRepositoryMock.Setup(x => x.HasReservationOnDate(studentId, pickupDate)).Returns(false);

            // Act
            var result = _packageService.ReservePackage(packageId, studentId);

            // Assert
            result.Success.Should().BeTrue();
            result.Message.Should().Contain("succesvol");

            _reservationRepositoryMock.Verify(x => x.Add(It.IsAny<Reservation>()), Times.Once);
        }
        [Fact]
        public void ReservePackage_StudentUnder18ReservingAdultPackage_ShouldReturnError()
        {
            // Arrange
            var packageId = 1;
            var studentId = 1;
            var pickupDate = DateTime.Now.Date.AddDays(1);

            var package = new Package
            {
                PackageId = packageId,
                Name = "Adult Package with Beer",
                PickupTime = pickupDate.AddHours(18),
                ExpirationTime = pickupDate.AddHours(20),
                Is18Plus = true,
                City = City.Breda,
                MealType = MealType.WarmeAvondmaaltijd,
                Price = 6.50m,
                PackageProducts = new List<PackageProducts>(),
                Canteen = new Canteen { CanteenId = 1, City = City.Breda, Location = Location.LA }
            };

            var minorStudent = new Student
            {
                StudentId = studentId,
                Name = "Minor Student",
                Email = "minor@avans.nl",
                BirthDate = new DateTime(2010, 1, 1),
                StudentNumber = "123456",
                StudyCity = City.Breda,
                NoShowCount = 0
            };

            _packageRepositoryMock.Setup(x => x.GetPackageWithDetails(packageId)).Returns(package);
            _studentRepositoryMock.Setup(x => x.GetById(studentId)).Returns(minorStudent);
            _packageRepositoryMock.Setup(x => x.IsAvailable(packageId)).Returns(true);
            _studentRepositoryMock.Setup(x => x.HasReservationOnDate(studentId, pickupDate)).Returns(false);

            // Act
            var result = _packageService.ReservePackage(packageId, studentId);

            // Assert
            result.Success.Should().BeFalse();
            result.Message.Should().Contain("18 jaar of ouder");
        }

        [Fact]
        public void ReservePackage_PackageAlreadyReserved_ShouldReturnError()
        {
            // Arrange
            var packageId = 1;
            var studentId = 1;

            var package = new Package
            {
                PackageId = packageId,
                Name = "Test Package",
                PickupTime = DateTime.Now.AddDays(1),
                ExpirationTime = DateTime.Now.AddDays(1).AddHours(2),
                Is18Plus = false,
                City = City.Breda,
                MealType = MealType.Brood,
                Price = 5.00m,
                Canteen = new Canteen { CanteenId = 1, City = City.Breda, Location = Location.LA }
            };

            var student = new Student
            {
                StudentId = studentId,
                Name = "Test Student",
                Email = "test@avans.nl",
                BirthDate = new DateTime(2000, 1, 1),
                StudentNumber = "123456",
                StudyCity = City.Breda
            };

            _packageRepositoryMock.Setup(x => x.GetPackageWithDetails(packageId)).Returns(package);
            _studentRepositoryMock.Setup(x => x.GetById(studentId)).Returns(student);
            _packageRepositoryMock.Setup(x => x.IsAvailable(packageId)).Returns(false);

            // Act
            var result = _packageService.ReservePackage(packageId, studentId);

            // Assert
            result.Success.Should().BeFalse();
            result.Message.Should().Contain("gereserveerd");
        }

        [Fact]
        public void UpdatePackage_PackageIsReserved_ShouldReturnError()
        {
            // Arrange
            var packageId = 1;
            var reservedPackage = new Package
            {
                PackageId = packageId,
                Name = "Reserved Package",
                PickupTime = DateTime.Now.AddDays(1),
                ExpirationTime = DateTime.Now.AddDays(1).AddHours(2),
                ReservedByStudentId = 5,
                City = City.Breda,
                MealType = MealType.Brood,
                Price = 5.00m,
                CanteenId = 1,
                PackageProducts = new List<PackageProducts>(),
                Canteen = new Canteen { CanteenId = 1, City = City.Breda, Location = Location.LA }
            };

            var updatedPackage = new Package
            {
                PackageId = packageId,
                Name = "Updated Name",
                PickupTime = DateTime.Now.AddDays(1),
                ExpirationTime = DateTime.Now.AddDays(1).AddHours(2),
                City = City.Breda,
                MealType = MealType.Brood,
                Price = 6.00m,
                CanteenId = 1
            };

            _packageRepositoryMock.Setup(x => x.GetPackageWithDetails(packageId)).Returns(reservedPackage);

            // Act
            var result = _packageService.UpdatePackage(updatedPackage, new List<int> { 1, 2 });

            // Assert
            result.Success.Should().BeFalse();
            result.Message.Should().Contain("gereserveerd");
        }

        [Fact]
        public void DeletePackage_PackageIsReserved_ShouldReturnError()
        {
            // Arrange
            var packageId = 1;
            var reservedPackage = new Package
            {
                PackageId = packageId,
                Name = "Reserved Package",
                ReservedByStudentId = 5,
                PickupTime = DateTime.Now.AddDays(1),
                ExpirationTime = DateTime.Now.AddDays(1).AddHours(2),
                City = City.Breda,
                MealType = MealType.Brood,
                Price = 5.00m
            };

            _packageRepositoryMock.Setup(x => x.GetById(packageId)).Returns(reservedPackage);

            // Act
            var result = _packageService.DeletePackage(packageId);

            // Assert
            result.Success.Should().BeFalse();
            result.Message.Should().Contain("gereserveerd");
        }

        [Fact]
        public void CreatePackage_HotMealButCanteenDoesNotSupport_ShouldReturnError()
        {
            // Arrange
            var employeeId = 1;
            var canteenWithoutHotMeals = new Canteen
            {
                CanteenId = 1,
                City = City.Breda,
                Location = Location.LD,
                OffersHotMeals = false
            };

            var hotMealPackage = new Package
            {
                Name = "Hot Dinner",
                City = City.Breda,
                MealType = MealType.WarmeAvondmaaltijd,
                CanteenId = 1,
                PickupTime = DateTime.Now.AddDays(1).AddHours(18),
                ExpirationTime = DateTime.Now.AddDays(1).AddHours(20),
                Price = 7.00m
            };

            var productIds = new List<int> { 1, 2 };

            _canteenRepositoryMock.Setup(x => x.GetById(1)).Returns(canteenWithoutHotMeals);

            // Act
            var result = _packageService.CreatePackage(hotMealPackage, employeeId, productIds);

            // Assert
            result.Success.Should().BeFalse();
            result.Message.Should().Contain("biedt geen warme avondmaaltijden aan");
        }

        [Fact]
        public void CreatePackage_MoreThan2DaysInAdvance_ShouldReturnError()
        {
            // Arrange
            var employeeId = 1;
            var canteen = new Canteen
            {
                CanteenId = 1,
                City = City.Breda,
                Location = Location.LA,
                OffersHotMeals = true
            };

            var futurePackage = new Package
            {
                Name = "Future Package",
                City = City.Breda,
                MealType = MealType.Brood,
                CanteenId = 1,
                PickupTime = DateTime.Now.AddDays(3).AddHours(12),
                ExpirationTime = DateTime.Now.AddDays(3).AddHours(14),
                Price = 5.00m
            };

            var productIds = new List<int> { 1, 2 };

            _canteenRepositoryMock.Setup(x => x.GetById(1)).Returns(canteen);
            _productRepositoryMock.Setup(x => x.GetAll()).Returns(new List<Product>
            {
                new Product { ProductId = 1, Name = "Product 1", ContainsAlcohol = false },
                new Product { ProductId = 2, Name = "Product 2", ContainsAlcohol = false }
            });

            // Act
            var result = _packageService.CreatePackage(futurePackage, employeeId, productIds);

            // Assert
            result.Success.Should().BeFalse();
            result.Message.Should().Contain("2 dagen vooruit");
        }

        [Fact]
        public void CreatePackage_ValidPackage_ShouldSucceed()
        {
            // Arrange
            var employeeId = 1;
            var canteen = new Canteen
            {
                CanteenId = 1,
                City = City.Breda,
                Location = Location.LA,
                OffersHotMeals = true
            };

            var validPackage = new Package
            {
                Name = "Valid Package",
                City = City.Breda,
                MealType = MealType.Brood,
                CanteenId = 1,
                PickupTime = DateTime.Now.AddDays(1).AddHours(12),
                ExpirationTime = DateTime.Now.AddDays(1).AddHours(14),
                Price = 5.00m
            };

            var productIds = new List<int> { 1, 2 };
            var products = new List<Product>
            {
                new Product { ProductId = 1, Name = "Product 1", ContainsAlcohol = false },
                new Product { ProductId = 2, Name = "Product 2", ContainsAlcohol = false }
            };

            _canteenRepositoryMock.Setup(x => x.GetById(1)).Returns(canteen);
            _productRepositoryMock.Setup(x => x.GetAll()).Returns(products);

            // Act
            var result = _packageService.CreatePackage(validPackage, employeeId, productIds);

            // Assert
            result.Success.Should().BeTrue();
            _packageRepositoryMock.Verify(x => x.Add(It.IsAny<Package>()), Times.Once);
        }

        [Fact]
        public void ReservePackage_StudentWithTooManyNoShows_ShouldReturnError()
        {
            // Arrange
            var packageId = 1;
            var studentId = 1;
            var pickupDate = DateTime.Now.Date.AddDays(1);

            var package = new Package
            {
                PackageId = packageId,
                Name = "Test Package",
                PickupTime = pickupDate.AddHours(12),
                ExpirationTime = pickupDate.AddHours(14),
                Is18Plus = false,
                City = City.Breda,
                MealType = MealType.Brood,
                Price = 5.00m,
                PackageProducts = new List<PackageProducts>(),
                Canteen = new Canteen { CanteenId = 1, City = City.Breda, Location = Location.LA }
            };

            var studentWithNoShows = new Student
            {
                StudentId = studentId,
                Name = "Bad Student",
                Email = "bad@avans.nl",
                BirthDate = new DateTime(2000, 1, 1),
                StudentNumber = "123456",
                StudyCity = City.Breda,
                NoShowCount = 3
            };

            _packageRepositoryMock.Setup(x => x.GetPackageWithDetails(packageId)).Returns(package);
            _studentRepositoryMock.Setup(x => x.GetById(studentId)).Returns(studentWithNoShows);
            _packageRepositoryMock.Setup(x => x.IsAvailable(packageId)).Returns(true);
            _studentRepositoryMock.Setup(x => x.HasReservationOnDate(studentId, pickupDate)).Returns(false);

            // Act
            var result = _packageService.ReservePackage(packageId, studentId);

            // Assert
            result.Success.Should().BeFalse();
            result.Message.Should().Contain("no-shows");
        }
    }
}