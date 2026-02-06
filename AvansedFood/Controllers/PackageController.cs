using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Application.Services;
using AvansedFood.Web.ViewModels;
using Domain.Models;

namespace AvansedFood.Web.Controllers
{
    [Authorize(Roles = "Student")]
    public class PackageController : Controller
    {
        private readonly IPackageService _packageService;
        private readonly IStudentService _studentService;

        public PackageController(
            IPackageService packageService,
            IStudentService studentService)
        {
            _packageService = packageService;
            _studentService = studentService;
        }

        private int? GetCurrentStudentId()
        {
            var email = User.Identity?.Name;
            if (string.IsNullOrEmpty(email)) return null;

            var student = _studentService.GetStudentByEmail(email);
            return student?.StudentId;
        }

        [HttpGet]
        public IActionResult Index(City? city, MealType? mealType)
        {
            var packages = _packageService.GetAvailablePackages(city, mealType);

            var viewModel = new PackageListViewModel
            {
                SelectedCity = city,
                SelectedMealType = mealType,
                Packages = packages.Select(p => new PackageViewModel
                {
                    PackageId = p.PackageId,
                    Name = p.Name,
                    City = p.City,
                    MealType = p.MealType,
                    CanteenName = $"{p.Canteen.City} - {p.Canteen.Location}",
                    CanteenLocation = p.Canteen.Location,
                    PickupTime = p.PickupTime,
                    ExpirationTime = p.ExpirationTime,
                    Price = p.Price,
                    Is18Plus = p.Is18Plus,
                    IsReserved = false,
                    Products = p.PackageProducts.Select(pp => new ProductViewModel
                    {
                        ProductId = pp.Product.ProductId,
                        Name = pp.Product.Name,
                        ContainsAlcohol = pp.Product.ContainsAlcohol,
                        PhotoUrl = pp.Product.PhotoUrl
                    }).ToList()
                }).ToList()
            };

            return View(viewModel);
        }

        [HttpGet]
        public IActionResult MyReservations()
        {
            var studentId = GetCurrentStudentId();
            if (studentId == null)
            {
                TempData["ErrorMessage"] = "Student profile not found. Please contact support.";
                return RedirectToAction("Index");
            }

            var packages = _packageService.GetReservedPackagesByStudent(studentId.Value);

            var viewModel = packages.Select(p => new PackageViewModel
            {
                PackageId = p.PackageId,
                Name = p.Name,
                City = p.City,
                MealType = p.MealType,
                CanteenName = $"{p.Canteen.City} - {p.Canteen.Location}",
                CanteenLocation = p.Canteen.Location,
                PickupTime = p.PickupTime,
                ExpirationTime = p.ExpirationTime,
                Price = p.Price,
                Is18Plus = p.Is18Plus,
                IsReserved = true,
                Products = p.PackageProducts.Select(pp => new ProductViewModel
                {
                    ProductId = pp.Product.ProductId,
                    Name = pp.Product.Name,
                    ContainsAlcohol = pp.Product.ContainsAlcohol,
                    PhotoUrl = pp.Product.PhotoUrl
                }).ToList()
            }).ToList();

            return View(viewModel);
        }

        [HttpGet]
        public IActionResult Details(int id)
        {
            var package = _packageService.GetPackageDetails(id);

            if (package == null)
            {
                return NotFound();
            }

            var viewModel = new PackageViewModel
            {
                PackageId = package.PackageId,
                Name = package.Name,
                City = package.City,
                MealType = package.MealType,
                CanteenName = $"{package.Canteen.City} - {package.Canteen.Location}",
                CanteenLocation = package.Canteen.Location,
                PickupTime = package.PickupTime,
                ExpirationTime = package.ExpirationTime,
                Price = package.Price,
                Is18Plus = package.Is18Plus,
                IsReserved = package.ReservedByStudentId != null,
                Products = package.PackageProducts.Select(pp => new ProductViewModel
                {
                    ProductId = pp.Product.ProductId,
                    Name = pp.Product.Name,
                    ContainsAlcohol = pp.Product.ContainsAlcohol,
                    PhotoUrl = pp.Product.PhotoUrl
                }).ToList()
            };

            return View(viewModel);
        }

        [HttpPost]
        public IActionResult Reserve(int id)
        {
            var studentId = GetCurrentStudentId();
            if (studentId == null)
            {
                TempData["ErrorMessage"] = "Student profile not found. Please contact support.";
                return RedirectToAction("Index");
            }

            var result = _packageService.ReservePackage(id, studentId.Value);

            if (result.Success)
            {
                TempData["SuccessMessage"] = result.Message;
                return RedirectToAction(nameof(MyReservations));
            }
            else
            {
                TempData["ErrorMessage"] = result.Message;
                return RedirectToAction(nameof(Details), new { id });
            }
        }
    }
}