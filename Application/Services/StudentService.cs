using Domain.Models;
using Domain.Repositories;

namespace Application.Services
{
    public class StudentService : IStudentService
    {
        private readonly IStudentRepository _studentRepository;
        private readonly IPackageRepository _packageRepository;

        public StudentService(
            IStudentRepository studentRepository,
            IPackageRepository packageRepository)
        {
            _studentRepository = studentRepository;
            _packageRepository = packageRepository;
        }

        public Student? GetStudentById(int id)
        {
            return _studentRepository.GetById(id);
        }

        public Student? GetStudentByEmail(string email)
        {
            return _studentRepository.GetByEmail(email);
        }

        public (bool Success, string Message) RegisterStudent(Student student)
        {
            if (student.GetAge() < 16)
            {
                return (false, "Je moet minimaal 16 jaar oud zijn om te registreren.");
            }

            if (student.BirthDate > DateTime.Today)
            {
                return (false, "Geboortedatum kan niet in de toekomst liggen.");
            }

            if (_studentRepository.GetByEmail(student.Email) != null)
            {
                return (false, "Dit e-mailadres is al geregistreerd.");
            }

            if (_studentRepository.GetByStudentNumber(student.StudentNumber) != null)
            {
                return (false, "Dit studentnummer is al geregistreerd.");
            }

            _studentRepository.Add(student);
            return (true, "Registratie succesvol!");
        }

        public bool CanReserveAdultPackage(int studentId, int packageId)
        {
            var student = _studentRepository.GetById(studentId);
            var package = _packageRepository.GetById(packageId);

            if (student == null || package == null)
                return false;

            if (!package.Is18Plus)
                return true;

            return student.IsAdult(package.PickupTime);
        }

        public bool CanReservePackageOnDate(int studentId, DateTime pickupDate)
        {
            return !_studentRepository.HasReservationOnDate(studentId, pickupDate);
        }

        public bool IsAllowedToReserve(int studentId)
        {
            var student = _studentRepository.GetById(studentId);
            return student?.CanReserve() ?? false;
        }
        public void IncrementNoShowCount(int studentId)
        {
            var student = _studentRepository.GetById(studentId);
            if (student != null)
            {
                student.NoShowCount++;
                _studentRepository.Update(student);
            }
        }
    }
}