using Domain.Models;
using Domain.Repositories;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class ReservationRepository : IReservationRepository
    {
        private readonly AppDbContext _context;

        public ReservationRepository(AppDbContext context)
        {
            _context = context;
        }

        public Reservation? GetById(int id)
        {
            return _context.Reservations
                .Include(r => r.Student)
                .Include(r => r.Package)
                    .ThenInclude(p => p.Canteen)
                .FirstOrDefault(r => r.ReservationId == id);
        }

        public IEnumerable<Reservation> GetByStudent(int studentId)
        {
            return _context.Reservations
                .Include(r => r.Package)
                    .ThenInclude(p => p.Canteen)
                .Where(r => r.StudentId == studentId)
                .OrderByDescending(r => r.CreatedAt)
                .ToList();
        }

        public IEnumerable<Reservation> GetByPackage(int packageId)
        {
            return _context.Reservations
                .Include(r => r.Student)
                .Where(r => r.PackageId == packageId)
                .ToList();
        }

        public IEnumerable<Reservation> GetUpcomingReservationsByStudent(int studentId)
        {
            return _context.Reservations
                .Include(r => r.Package)
                    .ThenInclude(p => p.Canteen)
                .Where(r => r.StudentId == studentId &&
                           r.Package.PickupTime > DateTime.Now)
                .OrderBy(r => r.Package.PickupTime)
                .ToList();
        }

        public bool ExistsForPackage(int packageId)
        {
            return _context.Reservations.Any(r => r.PackageId == packageId);
        }

        public void Add(Reservation reservation)
        {
            _context.Reservations.Add(reservation);
            _context.SaveChanges();
        }

        public void Update(Reservation reservation)
        {
            _context.Reservations.Update(reservation);
            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            var reservation = GetById(id);
            if (reservation != null)
            {
                _context.Reservations.Remove(reservation);
                _context.SaveChanges();
            }
        }
    }
}