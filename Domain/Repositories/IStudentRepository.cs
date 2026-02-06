using Domain.Models;

namespace Domain.Repositories
{
    public interface IStudentRepository
    {
        IEnumerable<Student> GetAll();
        Student? GetById(int id);

        Student? GetByEmail(string email);
        Student? GetByStudentNumber(string studentNumber);
        bool HasReservationOnDate(int studentId, DateTime date);
        int GetReservationCountOnDate(int studentId, DateTime date);

        void Add(Student student);
        void Update(Student student);
        void Delete(int id);
    }
}