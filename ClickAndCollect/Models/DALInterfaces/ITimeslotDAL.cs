using ClickAndCollect.Models.Classes;

namespace ClickAndCollect.Models.DALInterfaces
{
    public interface ITimeslotDAL
    {
        Task<bool> AddTimeslotAsync(Timeslot t);
        Task<bool> UpdateTimeslotAsync(Timeslot t);
        Task<bool> RemoveTimeslotAsync(Timeslot t);
        Task<Timeslot> GetTimeslotAsync(int id);

        Task<List<Timeslot>> GetAllTimeslotsAsync();

        Task<List<Timeslot>> GetStoreTimeslotsAsync(int id);
    }
}
