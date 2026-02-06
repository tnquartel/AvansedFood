using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        public DbSet<Student> Students => Set<Student>();
        public DbSet<Product> Products => Set<Product>();
        public DbSet<Package> Packages => Set<Package>();
        public DbSet<Reservation> Reservations => Set<Reservation>();
        public DbSet<Canteen> Canteens => Set<Canteen>();
        public DbSet<CanteenEmployee> Employees => Set<CanteenEmployee>();

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<PackageProducts>()
                .HasKey(pp => new { pp.PackageId, pp.ProductId });

            builder.Entity<PackageProducts>()
                .HasOne(pp => pp.Package)
                .WithMany(p => p.PackageProducts)
                .HasForeignKey(pp => pp.PackageId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<PackageProducts>()
                .HasOne(pp => pp.Product)
                .WithMany(p => p.PackageProducts)
                .HasForeignKey(pp => pp.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Canteen>()
                .Property(c => c.City)
                .HasConversion<string>();

            builder.Entity<Canteen>()
                .Property(c => c.Location)
                .HasConversion<string>();

            builder.Entity<CanteenEmployee>()
                .HasOne(e => e.Canteen)
                .WithMany(c => c.Employees)
                .HasForeignKey(e => e.CanteenId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Package>(entity =>
            {
                entity.Property(p => p.Price)
                    .HasPrecision(10, 2);

                entity.Property(p => p.City)
                    .HasConversion<string>();

                entity.Property(p => p.MealType)
                    .HasConversion<string>();

                entity.HasOne(p => p.Canteen)
                    .WithMany(c => c.Packages)
                    .HasForeignKey(p => p.CanteenId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(p => p.ReservedByStudent)
                    .WithMany()
                    .HasForeignKey(p => p.ReservedByStudentId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .IsRequired(false);
            });

            builder.Entity<Student>()
                .Property(s => s.StudyCity)
                .HasConversion<string>();

            builder.Entity<Reservation>(entity =>
            {
                entity.HasKey(r => r.ReservationId);

                entity.HasOne(r => r.Student)
                    .WithMany(s => s.Reservations)
                    .HasForeignKey(r => r.StudentId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(r => r.Package)
                    .WithMany()
                    .HasForeignKey(r => r.PackageId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}