using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Application.Services;
using AvansedFood.Web.ViewModels;
using Domain.Models;
using Domain.Repositories;

namespace AvansedFood.Web.Controllers
{
    [Authorize(Roles = "CanteenEmployee")]
    public class CanteenController : Controller
    {
        private readonly IPackageService _packageService;
        private readonly ICanteenService _canteenService;
        private readonly IProductRepository _productRepository;

        public CanteenController(
            IPackageService packageService,
            ICanteenService canteenService,
            Domain.Repositories.IProductRepository productRepository)
        {
            _packageService = packageService;
            _canteenService = canteenService;
            _productRepository = productRepository;
        }

        // US_02: Overzicht van alle pakketten
        [HttpGet]
        public IActionResult Index()
        {
            // TODO: Haal canteenId op van ingelogde medewerker
            int canteenId = 1; // Vervang door echte canteen ID

            var packages = _packageService.GetPackagesByCanteen(canteenId);

            var viewModel = packages.Select(p => new PackageViewModel
            {
                PackageId = p.PackageId,
                Name = p.Name,
                City = p.City,
                MealType = p.MealType,
                CanteenName = $"{p.Canteen.City} - {p.Canteen.Location}",
                PickupTime = p.PickupTime,
                ExpirationTime = p.ExpirationTime,
                Price = p.Price,
                Is18Plus = p.Is18Plus,
                IsReserved = p.ReservedByStudentId != null
            }).ToList();

            return View(viewModel);
        }

        // US_02: Pakketten van andere kantines
        [HttpGet]
        public IActionResult AllPackages()
        {
            var packages = _packageService.GetAllPackages();

            var viewModel = packages.Select(p => new PackageViewModel
            {
                PackageId = p.PackageId,
                Name = p.Name,
                City = p.City,
                MealType = p.MealType,
                CanteenName = $"{p.Canteen.City} - {p.Canteen.Location}",
                PickupTime = p.PickupTime,
                ExpirationTime = p.ExpirationTime,
                Price = p.Price,
                Is18Plus = p.Is18Plus,
                IsReserved = p.ReservedByStudentId != null
            }).OrderBy(p => p.PickupTime).ToList();

            return View(viewModel);
        }

        // US_03: Pakket aanmaken - GET
        [HttpGet]
        public IActionResult Create()
        {
            // TODO: Haal canteenId op van ingelogde medewerker
            int canteenId = 1;

            var viewModel = new CreatePackageViewModel
            {
                AvailableCanteens = _canteenService.GetAllCanteens().ToList(),
                AvailableProducts = _productRepository.GetAll().ToList(),
                CanteenId = canteenId,
                PickupTime = DateTime.Now.AddHours(1),
                ExpirationTime = DateTime.Now.AddHours(3)
            };

            return View(viewModel);
        }

        // US_03: Pakket aanmaken - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(CreatePackageViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                viewModel.AvailableCanteens = _canteenService.GetAllCanteens().ToList();
                viewModel.AvailableProducts = _productRepository.GetAll().ToList();
                return View(viewModel);
            }

            // TODO: Haal employeeId op van ingelogde medewerker
            int employeeId = 1;

            var package = new Package
            {
                Name = viewModel.Name,
                City = viewModel.City,
                MealType = viewModel.MealType,
                CanteenId = viewModel.CanteenId,
                PickupTime = viewModel.PickupTime,
                ExpirationTime = viewModel.ExpirationTime,
                Price = viewModel.Price
            };

            var result = _packageService.CreatePackage(package, employeeId, viewModel.SelectedProductIds);

            if (result.Success)
            {
                TempData["SuccessMessage"] = result.Message;
                return RedirectToAction(nameof(Index));
            }
            else
            {
                ModelState.AddModelError("", result.Message);
                viewModel.AvailableCanteens = _canteenService.GetAllCanteens().ToList();
                viewModel.AvailableProducts = _productRepository.GetAll().ToList();
                return View(viewModel);
            }
        }

        // US_03: Pakket bewerken - GET
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var package = _packageService.GetPackageDetails(id);

            if (package == null)
            {
                return NotFound();
            }

            var viewModel = new CreatePackageViewModel
            {
                Name = package.Name,
                City = package.City,
                MealType = package.MealType,
                CanteenId = package.CanteenId,
                PickupTime = package.PickupTime,
                ExpirationTime = package.ExpirationTime,
                Price = package.Price,
                SelectedProductIds = package.PackageProducts.Select(pp => pp.ProductId).ToList(),
                AvailableCanteens = _canteenService.GetAllCanteens().ToList(),
                AvailableProducts = _productRepository.GetAll().ToList()
            };

            return View(viewModel);
        }

        // US_03: Pakket bewerken - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, CreatePackageViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                viewModel.AvailableCanteens = _canteenService.GetAllCanteens().ToList();
                viewModel.AvailableProducts = _productRepository.GetAll().ToList();
                return View(viewModel);
            }

            var package = new Package
            {
                PackageId = id,
                Name = viewModel.Name,
                City = viewModel.City,
                MealType = viewModel.MealType,
                CanteenId = viewModel.CanteenId,
                PickupTime = viewModel.PickupTime,
                ExpirationTime = viewModel.ExpirationTime,
                Price = viewModel.Price
            };

            var result = _packageService.UpdatePackage(package, viewModel.SelectedProductIds);

            if (result.Success)
            {
                TempData["SuccessMessage"] = result.Message;
                return RedirectToAction(nameof(Index));
            }
            else
            {
                ModelState.AddModelError("", result.Message);
                viewModel.AvailableCanteens = _canteenService.GetAllCanteens().ToList();
                viewModel.AvailableProducts = _productRepository.GetAll().ToList();
                return View(viewModel);
            }
        }

        // US_03: Pakket verwijderen
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            var result = _packageService.DeletePackage(id);

            if (result.Success)
            {
                TempData["SuccessMessage"] = result.Message;
            }
            else
            {
                TempData["ErrorMessage"] = result.Message;
            }

            return RedirectToAction(nameof(Index));
        }
    }
}