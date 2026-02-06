using Application.Services;
using Domain.Models;
using Domain.Repositories;
using FluentAssertions;
using Moq;
using Xunit;

namespace AvansedFood.Tests.Services
{
    public class CanteenServiceTests
    {
        private readonly Mock<ICanteenRepository> _canteenRepositoryMock;
        private readonly CanteenService _canteenService;

        public CanteenServiceTests()
        {
            _canteenRepositoryMock = new Mock<ICanteenRepository>();
            _canteenService = new CanteenService(_canteenRepositoryMock.Object);
        }

        [Fact]
        public void GetCanteenById_ValidId_ReturnsCanteen()
        {
            // Arrange
            var canteenId = 1;
            var expectedCanteen = new Canteen
            {
                CanteenId = canteenId,
                City = City.Breda,
                Location = Location.LA,
                OffersHotMeals = true
            };

            _canteenRepositoryMock
                .Setup(x => x.GetById(canteenId))
                .Returns(expectedCanteen);

            // Act
            var result = _canteenService.GetCanteenById(canteenId);

            // Assert
            result.Should().NotBeNull();
            result.Should().Be(expectedCanteen);
            result!.CanteenId.Should().Be(canteenId);
            result.City.Should().Be(City.Breda);
            result.Location.Should().Be(Location.LA);
        }

        [Fact]
        public void GetCanteenById_InvalidId_ReturnsNull()
        {
            // Arrange
            var invalidId = 999;

            _canteenRepositoryMock
                .Setup(x => x.GetById(invalidId))
                .Returns((Canteen?)null);

            // Act
            var result = _canteenService.GetCanteenById(invalidId);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public void GetAllCanteens_ReturnsAllCanteens()
        {
            // Arrange
            var canteens = new List<Canteen>
            {
                new Canteen { CanteenId = 1, City = City.Breda, Location = Location.LA, OffersHotMeals = true },
                new Canteen { CanteenId = 2, City = City.Breda, Location = Location.LD, OffersHotMeals = false },
                new Canteen { CanteenId = 3, City = City.Tilburg, Location = Location.TA, OffersHotMeals = true }
            };

            _canteenRepositoryMock
                .Setup(x => x.GetAll())
                .Returns(canteens);

            // Act
            var result = _canteenService.GetAllCanteens();

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(3);
            result.Should().Contain(c => c.City == City.Breda && c.Location == Location.LA);
            result.Should().Contain(c => c.City == City.Tilburg);
        }

        [Fact]
        public void GetCanteenByLocation_ValidLocation_ReturnsCanteen()
        {
            // Arrange
            var location = Location.LA;
            var expectedCanteen = new Canteen
            {
                CanteenId = 1,
                City = City.Breda,
                Location = location,
                OffersHotMeals = true
            };

            _canteenRepositoryMock
                .Setup(x => x.GetByLocation(location))
                .Returns(expectedCanteen);

            // Act
            var result = _canteenService.GetCanteenByLocation(location);

            // Assert
            result.Should().NotBeNull();
            result.Should().Be(expectedCanteen);
            result!.Location.Should().Be(location);
        }

        [Fact]
        public void GetCanteenByLocation_InvalidLocation_ReturnsNull()
        {
            // Arrange
            var location = Location.HA;

            _canteenRepositoryMock
                .Setup(x => x.GetByLocation(location))
                .Returns((Canteen?)null);

            // Act
            var result = _canteenService.GetCanteenByLocation(location);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public void CanOfferHotMeal_CanteenSupportsHotMeals_ReturnsTrue()
        {
            // Arrange
            var canteenId = 1;
            var canteen = new Canteen
            {
                CanteenId = canteenId,
                City = City.Breda,
                Location = Location.LA,
                OffersHotMeals = true
            };

            _canteenRepositoryMock
                .Setup(x => x.GetById(canteenId))
                .Returns(canteen);

            // Act
            var result = _canteenService.CanOfferHotMeal(canteenId);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void CanOfferHotMeal_CanteenDoesNotSupportHotMeals_ReturnsFalse()
        {
            // Arrange
            var canteenId = 2;
            var canteen = new Canteen
            {
                CanteenId = canteenId,
                City = City.Breda,
                Location = Location.LD,
                OffersHotMeals = false
            };

            _canteenRepositoryMock
                .Setup(x => x.GetById(canteenId))
                .Returns(canteen);

            // Act
            var result = _canteenService.CanOfferHotMeal(canteenId);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void CanOfferHotMeal_CanteenNotFound_ReturnsFalse()
        {
            // Arrange
            var invalidId = 999;

            _canteenRepositoryMock
                .Setup(x => x.GetById(invalidId))
                .Returns((Canteen?)null);

            // Act
            var result = _canteenService.CanOfferHotMeal(invalidId);

            // Assert
            result.Should().BeFalse();
        }
    }
}