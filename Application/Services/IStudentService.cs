using Domain.Models;

namespace Application.Services
{
    public interface IStudentService
    {
        Student? GetStudentById(int id);
        Student? GetStudentByEmail(string email);
        (bool Success, string Message) RegisterStudent(Student student);

        bool CanReserveAdultPackage(int studentId, int packageId);

        bool CanReservePackageOnDate(int studentId, DateTime pickupDate);

        bool IsAllowedToReserve(int studentId);
        void IncrementNoShowCount(int studentId);
    }
}