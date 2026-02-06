using Domain.Models;
using Domain.Repositories;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class StudentRepository : IStudentRepository
    {
        private readonly AppDbContext _context;

        public StudentRepository(AppDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Student> GetAll()
            => _context.Students.ToList();

        public Student? GetById(int id)
            => _context.Students.Find(id);

        public Student? GetByEmail(string email)
            => _context.Students.FirstOrDefault(s => s.Email == email);

        public Student? GetByStudentNumber(string studentNumber)
            => _context.Students.FirstOrDefault(s => s.StudentNumber == studentNumber);

        public bool HasReservationOnDate(int studentId, DateTime date)
        {
            var targetDate = date.Date;

            return _context.Reservations
                .Include(r => r.Package)
                .Any(r => r.StudentId == studentId
                       && r.Package.PickupTime.Date == targetDate);
        }

        public int GetReservationCountOnDate(int studentId, DateTime date)
        {
            var targetDate = date.Date;

            return _context.Reservations
                .Include(r => r.Package)
                .Count(r => r.StudentId == studentId
                         && r.Package.PickupTime.Date == targetDate);
        }

        public void Add(Student student)
        {
            _context.Students.Add(student);
            _context.SaveChanges();
        }

        public void Update(Student student)
        {
            _context.Students.Update(student);
            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            var s = GetById(id);
            if (s != null)
            {
                _context.Students.Remove(s);
                _context.SaveChanges();
            }
        }
    }
}