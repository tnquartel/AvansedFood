using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data
{
    public static class DatabaseSeeder
    {
        public static void SeedDatabase(AppDbContext context)
        {
            // Check if data already exists
            if (context.Canteens.Any())
            {
                return; // Database already seeded
            }

            // Seed Canteens
            var canteens = new List<Canteen>
            {
                new Canteen
                {
                    City = City.Breda,
                    Location = Location.LA,
                    OffersHotMeals = true
                },
                new Canteen
                {
                    City = City.Breda,
                    Location = Location.LD,
                    OffersHotMeals = false
                },
                new Canteen
                {
                    City = City.DenBosch,
                    Location = Location.HA,
                    OffersHotMeals = true
                },
                new Canteen
                {
                    City = City.Tilburg,
                    Location = Location.TA,
                    OffersHotMeals = true
                }
            };

            context.Canteens.AddRange(canteens);
            context.SaveChanges();

            // Seed Products
            var products = new List<Product>
            {
                new Product { Name = "Broodje Kaas", ContainsAlcohol = false, PhotoUrl = "/images/broodje-kaas.jpg" },
                new Product { Name = "Broodje Ham", ContainsAlcohol = false, PhotoUrl = "/images/broodje-ham.jpg" },
                new Product { Name = "Croissant", ContainsAlcohol = false, PhotoUrl = "/images/croissant.jpg" },
                new Product { Name = "Appelsap", ContainsAlcohol = false, PhotoUrl = "/images/appelsap.jpg" },
                new Product { Name = "Koffie", ContainsAlcohol = false, PhotoUrl = "/images/koffie.jpg" },
                new Product { Name = "Pasta Bolognese", ContainsAlcohol = false, PhotoUrl = "/images/pasta.jpg" },
                new Product { Name = "Lasagne", ContainsAlcohol = false, PhotoUrl = "/images/lasagne.jpg" },
                new Product { Name = "Salade", ContainsAlcohol = false, PhotoUrl = "/images/salade.jpg" },
                new Product { Name = "Friet", ContainsAlcohol = false, PhotoUrl = "/images/friet.jpg" },
                new Product { Name = "Hamburger", ContainsAlcohol = false, PhotoUrl = "/images/hamburger.jpg" },
                new Product { Name = "Bier", ContainsAlcohol = true, PhotoUrl = "/images/bier.jpg" },
                new Product { Name = "Wijn", ContainsAlcohol = true, PhotoUrl = "/images/wijn.jpg" }
            };

            context.Products.AddRange(products);
            context.SaveChanges();

            // Seed Students
            var students = new List<Student>
            {
                new Student
                {
                    StudentNumber = "2000001",
                    Name = "Thomas de Student",
                    BirthDate = new DateTime(2000, 5, 15),
                    Email = "student@avans.nl",
                    PhoneNumber = "0612345678",
                    StudyCity = City.Breda,
                    NoShowCount = 0
                },
                new Student
                {
                    StudentNumber = "2000002",
                    Name = "Lisa Jansen",
                    BirthDate = new DateTime(2001, 8, 20),
                    Email = "lisa.jansen@avans.nl",
                    PhoneNumber = "0687654321",
                    StudyCity = City.Tilburg,
                    NoShowCount = 0
                },
                new Student
                {
                    StudentNumber = "2000003",
                    Name = "Mark Bakker",
                    BirthDate = new DateTime(2016, 3, 10),
                    Email = "mark.bakker@avans.nl",
                    PhoneNumber = "0698765432",
                    StudyCity = City.DenBosch,
                    NoShowCount = 0
                }
            };

            context.Students.AddRange(students);
            context.SaveChanges();

            // Seed Canteen Employees
            var employees = new List<CanteenEmployee>
            {
                new CanteenEmployee
                {
                    PersonnelNumber = "EMP001",
                    Name = "Jan de Medewerker",
                    CanteenId = canteens[0].CanteenId
                },
                new CanteenEmployee
                {
                    PersonnelNumber = "EMP002",
                    Name = "Marie Pietersen",
                    CanteenId = canteens[2].CanteenId
                }
            };

            context.Employees.AddRange(employees);
            context.SaveChanges();

            var packages = new List<Package>
            {
                new Package
                {
                    Name = "Brood Lunch Pakket",
                    City = City.Breda,
                    MealType = MealType.Brood,
                    CanteenId = canteens[0].CanteenId,
                    PickupTime = DateTime.Now.AddHours(3),
                    ExpirationTime = DateTime.Now.AddHours(5),
                    Price = 3.50m,
                    Is18Plus = false
                },
                new Package
                {
                    Name = "Avondmaaltijd Deluxe",
                    City = City.Breda,
                    MealType = MealType.WarmeAvondmaaltijd,
                    CanteenId = canteens[0].CanteenId,
                    PickupTime = DateTime.Now.AddDays(1).AddHours(-2),
                    ExpirationTime = DateTime.Now.AddDays(1),
                    Price = 6.50m,
                    Is18Plus = true
                },
                new Package
                {
                    Name = "Drank Pakket",
                    City = City.Tilburg,
                    MealType = MealType.Drank,
                    CanteenId = canteens[3].CanteenId,
                    PickupTime = DateTime.Now.AddHours(2),
                    ExpirationTime = DateTime.Now.AddHours(4),
                    Price = 2.00m,
                    Is18Plus = false,
                    ReservedByStudentId = students[1].StudentId
                },
                new Package
                {
                    Name = "Snack Combo",
                    City = City.DenBosch,
                    MealType = MealType.Brood,
                    CanteenId = canteens[2].CanteenId,
                    PickupTime = DateTime.Now.AddHours(4),
                    ExpirationTime = DateTime.Now.AddHours(6),
                    Price = 4.00m,
                    Is18Plus = false
                }
            };

            context.Packages.AddRange(packages);
            context.SaveChanges();

            var packageProducts = new List<PackageProducts>
            {
                new PackageProducts { PackageId = packages[0].PackageId, ProductId = products[0].ProductId },
                new PackageProducts { PackageId = packages[0].PackageId, ProductId = products[2].ProductId },
                new PackageProducts { PackageId = packages[0].PackageId, ProductId = products[3].ProductId },

                new PackageProducts { PackageId = packages[1].PackageId, ProductId = products[5].ProductId },
                new PackageProducts { PackageId = packages[1].PackageId, ProductId = products[7].ProductId },
                new PackageProducts { PackageId = packages[1].PackageId, ProductId = products[10].ProductId },

                new PackageProducts { PackageId = packages[2].PackageId, ProductId = products[3].ProductId },
                new PackageProducts { PackageId = packages[2].PackageId, ProductId = products[4].ProductId },

                new PackageProducts { PackageId = packages[3].PackageId, ProductId = products[8].ProductId },
                new PackageProducts { PackageId = packages[3].PackageId, ProductId = products[9].ProductId }
            };

            context.AddRange(packageProducts);
            context.SaveChanges();

            var reservation = new Reservation
            {
                PackageId = packages[2].PackageId,
                StudentId = students[1].StudentId,
                IsPickedUp = false,
                CreatedAt = DateTime.Now
            };

            context.Reservations.Add(reservation);
            context.SaveChanges();

            Console.WriteLine("Database seeded successfully!");
        }
    }
}