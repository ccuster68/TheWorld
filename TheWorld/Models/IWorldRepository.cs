using System.Collections.Generic;
using System.Threading.Tasks;

namespace TheWorld.Models
{
    public interface IWorldRepository
    {
        IEnumerable<Trip> GetAllTrips();

        void AddTrip(Trip trip);
        Trip GetTripByName(string tripName);
        Task<bool> SaveChangesAsync();
        void AddStop(string tripName, Stop newStop);
    }
}