using Domain.Models;
using Domain.Repositories;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class CanteenRepository : ICanteenRepository
    {
        private readonly AppDbContext _context;

        public CanteenRepository(AppDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Canteen> GetAll()
        {
            return _context.Canteens
                .Include(c => c.Employees)
                .Include(c => c.Packages)
                .ToList();
        }

        public Canteen? GetById(int id)
        {
            return _context.Canteens
                .Include(c => c.Employees)
                .Include(c => c.Packages)
                .FirstOrDefault(c => c.CanteenId == id);
        }

        public IEnumerable<Canteen> GetByCity(City city)
        {
            return _context.Canteens
                .Include(c => c.Employees)
                .Where(c => c.City == city)
                .ToList();
        }

        public Canteen? GetByLocation(Location location)
        {
            return _context.Canteens
                .Include(c => c.Employees)
                .FirstOrDefault(c => c.Location == location);
        }

        public void Add(Canteen canteen)
        {
            _context.Canteens.Add(canteen);
            _context.SaveChanges();
        }

        public void Update(Canteen canteen)
        {
            _context.Canteens.Update(canteen);
            _context.SaveChanges();
        }
    }
}