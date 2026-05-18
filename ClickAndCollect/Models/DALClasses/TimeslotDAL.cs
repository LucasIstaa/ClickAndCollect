using ClickAndCollect.Models.Classes;
using ClickAndCollect.Models.DALInterfaces;
using Microsoft.Data.SqlClient;

namespace ClickAndCollect.Models.DALClasses
{

    public class TimeslotDAL : ITimeslotDAL
    {

        private String connectionString;

        public TimeslotDAL(String conn)
        {
            this.connectionString = conn;
        }
        public Task<bool> AddTimeslotAsync(Timeslot t)
        {
            throw new NotImplementedException();
        }

        public Task<List<Timeslot>> GetAllTimeslotsAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<List<Timeslot>> GetStoreTimeslotsAsync(int storeId)
        {
            List<Timeslot> timeslots = new List<Timeslot>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(
                    @"SELECT 
                t.TimeslotId,
                t.start_,
                t.end_,
                COUNT(o.OrderId) AS OrderCount
              FROM Timeslot t
              LEFT JOIN Order_ o ON o.TimeslotId = t.TimeslotId
              WHERE t.StoreId = @storeId
              GROUP BY t.TimeslotId, t.start_, t.end_
              ORDER BY t.start_",
                    connection);

                cmd.Parameters.AddWithValue("@storeId", storeId);
                await connection.OpenAsync();

                using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        int tid = reader.GetInt32(0);
                        TimeOnly start = TimeOnly.FromTimeSpan(reader.GetTimeSpan(1));
                        TimeOnly end = TimeOnly.FromTimeSpan(reader.GetTimeSpan(2));
                        int orderCount = reader.GetInt32(3);

                        Store store = new Store(storeId);

                        Timeslot t = new Timeslot(tid, start, end, store, orderCount);

                        timeslots.Add(t);
                    }
                }
            }

            return timeslots;
        }




        public Task<Timeslot> GetTimeslotAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<bool> RemoveTimeslotAsync(Timeslot t)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateTimeslotAsync(Timeslot t)
        {
            throw new NotImplementedException();
        }
    }
}
