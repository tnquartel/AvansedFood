using AvansedFood.Web.GraphQL.Types;
using Application.Services;
using Domain.Models;
using Domain.Repositories;

namespace AvansedFood.Web.GraphQL.Queries
{
    public class PackageQueries
    {
        [UseProjection]
        [UseFiltering]
        [UseSorting]
        public IEnumerable<PackageType> GetPackages(
            [Service] IPackageService packageService,
            City? city = null,
            MealType? mealType = null)
        {
            var packages = packageService.GetAvailablePackages(city, mealType);

            return packages.Select(p => new PackageType
            {
                Id = p.PackageId,
                Name = p.Name,
                City = p.City.ToString(),
                MealType = p.MealType.ToString(),
                PickupTime = p.PickupTime,
                ExpirationTime = p.ExpirationTime,
                Price = p.Price,
                Is18Plus = p.Is18Plus,
                IsReserved = p.ReservedByStudentId != null,
                Canteen = p.Canteen != null ? new CanteenType
                {
                    Id = p.Canteen.CanteenId,
                    City = p.Canteen.City.ToString(),
                    Location = p.Canteen.Location.ToString(),
                    OffersHotMeals = p.Canteen.OffersHotMeals
                } : null,
                Products = p.PackageProducts.Select(pp => new ProductType
                {
                    Id = pp.Product.ProductId,
                    Name = pp.Product.Name,
                    ContainsAlcohol = pp.Product.ContainsAlcohol,
                    PhotoUrl = pp.Product.PhotoUrl
                }).ToList()
            }).ToList();
        }

        public PackageType? GetPackage(
            [Service] IPackageService packageService,
            int id)
        {
            var package = packageService.GetPackageDetails(id);

            if (package == null) return null;

            return new PackageType
            {
                Id = package.PackageId,
                Name = package.Name,
                City = package.City.ToString(),
                MealType = package.MealType.ToString(),
                PickupTime = package.PickupTime,
                ExpirationTime = package.ExpirationTime,
                Price = package.Price,
                Is18Plus = package.Is18Plus,
                IsReserved = package.ReservedByStudentId != null,
                Canteen = package.Canteen != null ? new CanteenType
                {
                    Id = package.Canteen.CanteenId,
                    City = package.Canteen.City.ToString(),
                    Location = package.Canteen.Location.ToString(),
                    OffersHotMeals = package.Canteen.OffersHotMeals
                } : null,
                Products = package.PackageProducts.Select(pp => new ProductType
                {
                    Id = pp.Product.ProductId,
                    Name = pp.Product.Name,
                    ContainsAlcohol = pp.Product.ContainsAlcohol,
                    PhotoUrl = pp.Product.PhotoUrl
                }).ToList()
            };
        }

        [UseProjection]
        public IEnumerable<PackageType> GetPackagesByCity(
            [Service] IPackageService packageService,
            City city)
        {
            var packages = packageService.GetAvailablePackages(city, null);

            return packages.Select(p => new PackageType
            {
                Id = p.PackageId,
                Name = p.Name,
                City = p.City.ToString(),
                MealType = p.MealType.ToString(),
                PickupTime = p.PickupTime,
                ExpirationTime = p.ExpirationTime,
                Price = p.Price,
                Is18Plus = p.Is18Plus,
                IsReserved = p.ReservedByStudentId != null,
                Canteen = p.Canteen != null ? new CanteenType
                {
                    Id = p.Canteen.CanteenId,
                    City = p.Canteen.City.ToString(),
                    Location = p.Canteen.Location.ToString(),
                    OffersHotMeals = p.Canteen.OffersHotMeals
                } : null,
                Products = p.PackageProducts.Select(pp => new ProductType
                {
                    Id = pp.Product.ProductId,
                    Name = pp.Product.Name,
                    ContainsAlcohol = pp.Product.ContainsAlcohol,
                    PhotoUrl = pp.Product.PhotoUrl
                }).ToList()
            }).ToList();
        }
    }
}