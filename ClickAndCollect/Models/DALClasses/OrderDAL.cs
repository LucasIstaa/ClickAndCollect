using ClickAndCollect.Models.Classes;
using Microsoft.Data.SqlClient;

namespace ClickAndCollect.Models.DALClasses
{
    public class OrderDAL : IOrderDAL
    {
        private String connectionString;

        public OrderDAL(String conn)
        {
            this.connectionString = conn;
        }

        public Task<bool> AddOrderAsync(Order o)
        {
            throw new NotImplementedException();
        }

        public Task<List<Order>> GetAllOrdersAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<List<Order>> GetClientOrdersAsync(int id)
        {
            List<Order> orders = new List<Order>();

            using(SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("SELECT o.OrderId, o.status, o.boxes, o.StoreId, o.TimeslotId,o.fetchdate,t.start_,t.end_ FROM dbo.Order_ o JOIN dbo.Timeslot t ON o.TimeslotId=t.TimeslotId WHERE UserId=@id", conn);
                cmd.Parameters.AddWithValue("id", id);
                await conn.OpenAsync();

                using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        Client cl = new Client(id,null,null,null, null, null, 0, null, null, 0);
                        int boxes = reader.GetInt32(reader.GetOrdinal("boxes"));
                        Enum.TryParse(reader.GetString(reader.GetOrdinal("status")), true, out OrderStatus status);
                        int orderid = reader.GetInt32(reader.GetOrdinal("OrderId"));
                        int storeid = reader.GetInt32(reader.GetOrdinal("StoreId"));
                        int timeslotid = reader.GetInt32(reader.GetOrdinal("TimeslotId"));
                        TimeOnly start = TimeOnly.FromTimeSpan(reader.GetTimeSpan(reader.GetOrdinal("start")));
                        TimeOnly end = TimeOnly.FromTimeSpan(reader.GetTimeSpan(reader.GetOrdinal("end")));
                        DateOnly fetchdate = DateOnly.FromDateTime(reader.GetDateTime(reader.GetOrdinal("fetchdate")));
                        Store store = new Store(storeid);
                        Timeslot tslot = new Timeslot(timeslotid,start,end,store);
                        List<OrderLine> lines = await GetOrderlinesAsync(orderid);
                        Order o = new Order(orderid, status, boxes, cl, store, tslot, lines, fetchdate);

                        orders.Add(o);
                    }
                }
            }
            return orders;
        }

        public Task<Order> GetOrderAsync(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<List<OrderLine>> GetOrderlinesAsync(int id)
        {
            List<OrderLine> orderlines = new List<OrderLine>();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("SELECT l.OrderLineId, l.quantity, p.name, p.price, p.ProductId,p.CategoryId FROM dbo.Order_line l  JOIN dbo.Product p ON"
                    +" l.ProductId=p.ProductId WHERE l.OrderId=@id", conn);
                cmd.Parameters.AddWithValue("id", id);
                await conn.OpenAsync();

                using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        int quantity = reader.GetInt32(reader.GetOrdinal("quantity"));
                        int ordlid = reader.GetInt32(reader.GetOrdinal("OrderLineId"));
                        int pid = reader.GetInt32(reader.GetOrdinal("ProductId"));
                        int catid = reader.GetInt32(reader.GetOrdinal("CategoryId"));
                        decimal price = reader.GetDecimal(reader.GetOrdinal("price"));
                        string pname = reader.GetString(reader.GetOrdinal("name"));

                        Product p  = new Product(pid,pname,price,new Category(catid,null));
                        OrderLine ol = new OrderLine(ordlid,quantity,p,null);

                        orderlines.Add(ol);

                    }
                }
            }

            return orderlines;
        }

        public Task<bool> RemoveOrderAsync(Order o)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateOrderAsync(Order o)
        {
            throw new NotImplementedException();
        }
    }
}
