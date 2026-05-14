using ClickAndCollect.Models.DALInterfaces;
using Microsoft.Data.SqlClient;

namespace ClickAndCollect.Models.DALClasses
{
    public class OrderDAL : IOrderDAL
    {
        private string connectionString;

        public OrderDAL(string conn)
        {
            this.connectionString = conn;
        }

        public async Task<List<Order>> GetTodayOrdersByStoreAsync(int storeId)
        {
            List<Order> orders = new List<Order>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(
                    @"SELECT o.OrderId, o.status, o.boxes, o.UserId,
                      u.firstname + ' ' + u.lastname AS ClientFullName,
                      t.start_ AS PickupTime
                      FROM dbo.Order_ o
                      INNER JOIN dbo.User_ u ON o.UserId = u.UserId
                      INNER JOIN dbo.Timeslot t ON o.TimeslotId = t.TimeslotId
                      WHERE o.StoreId = @storeId 
                      AND CAST(t.start_ AS DATE) = CAST(GETDATE() AS DATE)
                      AND o.status != 'Finalized'
                      ORDER BY t.start_",
                    connection);

                cmd.Parameters.AddWithValue("@storeId", storeId);
                await connection.OpenAsync();

                using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        Order o = new Order(
                            reader.GetInt32(reader.GetOrdinal("OrderId")),
                            reader.GetString(reader.GetOrdinal("status")),
                            reader.GetInt32(reader.GetOrdinal("boxes")),
                            reader.GetInt32(reader.GetOrdinal("UserId")),
                            reader.GetString(reader.GetOrdinal("ClientFullName")),
                            reader.GetDateTime(reader.GetOrdinal("PickupTime"))
                        );
                        orders.Add(o);
                    }
                }
            }
            return orders;
        }

        public async Task<bool> FinalizeOrderAsync(int orderId)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(
                    "UPDATE dbo.Order_ SET status = 'Finalized' WHERE OrderId = @orderId",
                    connection);

                cmd.Parameters.AddWithValue("@orderId", orderId);
                await connection.OpenAsync();

                int result = await cmd.ExecuteNonQueryAsync();
                return result > 0;
            }
        }
    }
}