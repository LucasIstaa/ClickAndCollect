using ClickAndCollect.Models.Classes;
using ClickAndCollect.Models.DALInterfaces;
using Microsoft.Data.SqlClient;

namespace ClickAndCollect.Models.DALClasses
{
    public class OrderlineDAL : IOrderlineDAL
    {
        private String connectionString;

        public OrderlineDAL(string conn)
        {
            this.connectionString = conn;
        }

        public async Task<bool> AddOrderlinesAsync(List<OrderLine> lines)
        {
            bool success = true;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                await conn.OpenAsync();

                foreach (OrderLine line in lines)
                {
                    SqlCommand cmd = new SqlCommand("INSERT INTO Order_line (OrderId, ProductId, Quantity) " +
                    "VALUES (@oid, @pid, @qty)", conn);

                    cmd.Parameters.AddWithValue("oid", line.Order.OrderId);
                    cmd.Parameters.AddWithValue("pid", line.Product.ProductId);
                    cmd.Parameters.AddWithValue("qty", line.Quantity);

                    int res = await cmd.ExecuteNonQueryAsync();

                    if (res <= 0)
                        success = false;
                }
            }

            return success;
        }
    }
}
