using Domain.Models;

namespace Domain.Repositories
{
    public interface IReservationRepository
    {
        Reservation? GetById(int id);
        IEnumerable<Reservation> GetByStudent(int studentId);
        IEnumerable<Reservation> GetByPackage(int packageId);
        void Add(Reservation reservation);
        void Update(Reservation reservation);
        void Delete(int id);
        bool ExistsForPackage(int packageId);
        IEnumerable<Reservation> GetUpcomingReservationsByStudent(int studentId);
    }
}