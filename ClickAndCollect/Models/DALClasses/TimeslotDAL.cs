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
                SqlCommand cmd = new SqlCommand(@"SELECT t.TimeslotId,t.start_,t.end_,COUNT(o.OrderId) AS OrderCount
                FROM Timeslot t LEFT JOIN Order_ o ON o.TimeslotId = t.TimeslotId WHERE t.StoreId = @storeId GROUP BY t.TimeslotId, t.start_, t.end_
                ORDER BY t.start_",connection);

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

                        Store store = new Store(storeId, null, null, -1, null, null, -1);

                        Timeslot t = new Timeslot(tid, start, end, store, orderCount);

                        timeslots.Add(t);
                    }
                }
            }

            return timeslots;
        }




        public async Task<Timeslot> GetTimeslotAsync(int id)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(@"
            SELECT t.TimeslotId, t.start_, t.end_, t.StoreId,
                   (SELECT COUNT(*) FROM Order_ o WHERE o.TimeslotId = t.TimeslotId) AS OrderCount
            FROM Timeslot t
            WHERE t.TimeslotId = @id",
                    connection);

                cmd.Parameters.AddWithValue("@id", id);

                await connection.OpenAsync();

                using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        int tid = reader.GetInt32(reader.GetOrdinal("TimeslotId"));
                        TimeOnly start = TimeOnly.FromTimeSpan(reader.GetTimeSpan(reader.GetOrdinal("start_")));
                        TimeOnly end = TimeOnly.FromTimeSpan(reader.GetTimeSpan(reader.GetOrdinal("end_")));
                        int storeId = reader.GetInt32(reader.GetOrdinal("StoreId"));
                        int orderCount = reader.GetInt32(reader.GetOrdinal("OrderCount"));

                        Store store = new Store(storeId, null, null, -1, null, null, -1);

                        return new Timeslot(tid, start, end, store, orderCount);
                    }
                }
            }

            return null;
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
