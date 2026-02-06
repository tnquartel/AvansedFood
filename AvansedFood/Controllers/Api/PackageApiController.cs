using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Application.Services;
using Domain.Models;

namespace AvansedFood.Web.Controllers.Api
{
    [Route("api/packages")]
    [ApiController]
    [AllowAnonymous]
    public class PackageApiController : ControllerBase
    {
        private readonly IPackageService _packageService;
        private readonly IStudentService _studentService;

        public PackageApiController(
            IPackageService packageService,
            IStudentService studentService)
        {
            _packageService = packageService;
            _studentService = studentService;
        }

        // GET: api/packages
        // Get all available packages
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<PackageDto>> GetAvailablePackages(
            [FromQuery] City? city = null,
            [FromQuery] MealType? mealType = null)
        {
            var packages = _packageService.GetAvailablePackages(city, mealType);

            var dtos = packages.Select(p => new PackageDto
            {
                Id = p.PackageId,
                Name = p.Name,
                City = p.City.ToString(),
                MealType = p.MealType.ToString(),
                CanteenLocation = p.Canteen.Location.ToString(),
                PickupTime = p.PickupTime,
                ExpirationTime = p.ExpirationTime,
                Price = p.Price,
                Is18Plus = p.Is18Plus,
                Products = p.PackageProducts.Select(pp => new ProductDto
                {
                    Id = pp.Product.ProductId,
                    Name = pp.Product.Name,
                    ContainsAlcohol = pp.Product.ContainsAlcohol,
                    PhotoUrl = pp.Product.PhotoUrl
                }).ToList()
            });

            return Ok(dtos);
        }

        // GET: api/packages/{id}
        // Get package details by ID
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<PackageDto> GetPackageById(int id)
        {
            var package = _packageService.GetPackageDetails(id);

            if (package == null)
            {
                return NotFound(new { message = "Package not found" });
            }

            var dto = new PackageDto
            {
                Id = package.PackageId,
                Name = package.Name,
                City = package.City.ToString(),
                MealType = package.MealType.ToString(),
                CanteenLocation = package.Canteen.Location.ToString(),
                PickupTime = package.PickupTime,
                ExpirationTime = package.ExpirationTime,
                Price = package.Price,
                Is18Plus = package.Is18Plus,
                Products = package.PackageProducts.Select(pp => new ProductDto
                {
                    Id = pp.Product.ProductId,
                    Name = pp.Product.Name,
                    ContainsAlcohol = pp.Product.ContainsAlcohol,
                    PhotoUrl = pp.Product.PhotoUrl
                }).ToList()
            };

            return Ok(dto);
        }

        // POST: api/packages/{id}/reserve
        // Reserve a package for the authenticated student
        [HttpPost("{id}/reserve")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public ActionResult ReservePackage(int id, [FromBody] ReservePackageRequest request)
        {
            // Validate package exists
            var package = _packageService.GetPackageDetails(id);
            if (package == null)
            {
                return NotFound(new { message = "Package not found" });
            }

            // Validate student exists
            var student = _studentService.GetStudentById(request.StudentId);
            if (student == null)
            {
                return BadRequest(new { message = "Invalid student ID" });
            }

            // Try to reserve
            var result = _packageService.ReservePackage(id, request.StudentId);

            if (result.Success)
            {
                return Ok(new
                {
                    message = result.Message,
                    packageId = id,
                    studentId = request.StudentId,
                    pickupTime = package.PickupTime
                });
            }
            else
            {
                // Determine status code based on error type
                if (result.Message.Contains("al gereserveerd") || result.Message.Contains("already reserved"))
                {
                    return Conflict(new { message = result.Message });
                }
                else
                {
                    return BadRequest(new { message = result.Message });
                }
            }
        }

        // GET: api/packages/my-reservations
        // Get all reservations for authenticated student
        [HttpGet("my-reservations")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<PackageDto>> GetMyReservations([FromQuery] int studentId)
        {
            var packages = _packageService.GetReservedPackagesByStudent(studentId);

            var dtos = packages.Select(p => new PackageDto
            {
                Id = p.PackageId,
                Name = p.Name,
                City = p.City.ToString(),
                MealType = p.MealType.ToString(),
                CanteenLocation = p.Canteen.Location.ToString(),
                PickupTime = p.PickupTime,
                ExpirationTime = p.ExpirationTime,
                Price = p.Price,
                Is18Plus = p.Is18Plus,
                Products = p.PackageProducts.Select(pp => new ProductDto
                {
                    Id = pp.Product.ProductId,
                    Name = pp.Product.Name,
                    ContainsAlcohol = pp.Product.ContainsAlcohol,
                    PhotoUrl = pp.Product.PhotoUrl
                }).ToList()
            });

            return Ok(dtos);
        }
    }

    // DTOs (Data Transfer Objects)
    public class PackageDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string MealType { get; set; } = string.Empty;
        public string CanteenLocation { get; set; } = string.Empty;
        public DateTime PickupTime { get; set; }
        public DateTime ExpirationTime { get; set; }
        public decimal Price { get; set; }
        public bool Is18Plus { get; set; }
        public List<ProductDto> Products { get; set; } = new();
    }

    public class ProductDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool ContainsAlcohol { get; set; }
        public string? PhotoUrl { get; set; }
    }

    public class ReservePackageRequest
    {
        public int StudentId { get; set; }
    }
}