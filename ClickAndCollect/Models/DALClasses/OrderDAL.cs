
using ClickAndCollect.Models.Classes;
using ClickAndCollect.Models.DALInterfaces;
using Microsoft.Data.SqlClient;

namespace ClickAndCollect.Models.DALClasses
{
    public class OrderDAL : IOrderDAL
    {
        private String connectionString;

        public OrderDAL(string conn)
        {
            this.connectionString = conn;
        }

        public async Task<bool> AddOrderAsync(Order o)
        {
            bool success = false;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("INSERT INTO Order_(status,boxes,StoreId,TimeslotId,UserId,fetchdate)"
                    + " OUTPUT INSERTED.OrderId VALUES(@stat,@box,@stid,@tid,@uid,@fdate)", conn);
                cmd.Parameters.AddWithValue("stat", o.Status.ToString());
                cmd.Parameters.AddWithValue("box", o.BoxesInvolved);
                cmd.Parameters.AddWithValue("stid", o.Store.StoreId);
                cmd.Parameters.AddWithValue("tid", o.Timeslot.TimeslotId);
                cmd.Parameters.AddWithValue("uid", o.Client.UserId);
                cmd.Parameters.AddWithValue("fdate", o.Fetchdate);
                await conn.OpenAsync();

                int newId = (int) await cmd.ExecuteScalarAsync();
                o.OrderId = newId;

                foreach (OrderLine line in o.Orderlines)
                {
                    line.Order = o;
                }
            }
            return success = o.OrderId > 0;
        }


        public Task<List<Order>> GetAllOrdersAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<List<Order>> GetClientOrdersAsync(int id)
        {
            List<Order> orders = new List<Order>();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(
                    @"SELECT o.OrderId, o.status, o.boxes, o.StoreId, o.TimeslotId, o.fetchdate,
                      t.start_, t.end_
                      FROM dbo.Order_ o
                      JOIN dbo.Timeslot t ON o.TimeslotId = t.TimeslotId
                      WHERE UserId = @id",
                    conn);
                cmd.Parameters.AddWithValue("id", id);
                await conn.OpenAsync();

                using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        Client cl = new Client(id, null, null, null, null, null, 0, null, null, 0);
                        int boxes = reader.GetInt32(reader.GetOrdinal("boxes"));
                        Enum.TryParse(reader.GetString(reader.GetOrdinal("status")), true, out OrderStatus status);
                        int orderid = reader.GetInt32(reader.GetOrdinal("OrderId"));
                        int storeid = reader.GetInt32(reader.GetOrdinal("StoreId"));
                        int timeslotid = reader.GetInt32(reader.GetOrdinal("TimeslotId"));
                        TimeOnly start = TimeOnly.FromTimeSpan(reader.GetTimeSpan(reader.GetOrdinal("start_")));
                        TimeOnly end = TimeOnly.FromTimeSpan(reader.GetTimeSpan(reader.GetOrdinal("end_")));
                        DateOnly fetchdate = DateOnly.FromDateTime(reader.GetDateTime(reader.GetOrdinal("fetchdate")));
                        Store store = new Store(storeid,null,null,-1,null,null,-1);
                        Timeslot tslot = new Timeslot(timeslotid, start, end, store);
                        Order o = new Order(orderid, status, boxes, cl, store, tslot, null, fetchdate);
                        o =  await GetOrderlinesAsync(o);

                        orders.Add(o);
                    }
                }
            }
            return orders;
        }

        public async Task<List<Order>> GetTodayOrdersByStoreAsync(int storeId)
        {
            List<Order> orders = new List<Order>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(
                    @"SELECT 
                o.OrderId, o.status, o.boxes, o.fetchdate,
                u.UserId, u.username, u.password, u.firstname, u.lastname,
                u.phonenumber, u.postalcode, u.Cityname, u.streetname, u.housenumber,
                t.TimeslotId, t.start_, t.end_,
                s.StoreId
              FROM dbo.Order_ o
              INNER JOIN dbo.User_ u ON o.UserId = u.UserId
              INNER JOIN dbo.Timeslot t ON o.TimeslotId = t.TimeslotId
              INNER JOIN dbo.Store s ON o.StoreId = s.StoreId
              WHERE o.StoreId = @storeId
                AND o.fetchdate = CAST(GETDATE() AS DATE)
                AND o.status != 'Finalized'
              ORDER BY t.start_",
                    connection);

                cmd.Parameters.AddWithValue("@storeId", storeId);
                await connection.OpenAsync();

                using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        orders.Add(ReadOrderListRow(reader));
                    }
                }
            }

            return orders;
        }

        private static Order ReadOrderListRow(SqlDataReader reader)
        {
            int storeid = reader.GetInt32(reader.GetOrdinal("StoreId"));
            Store store = new Store(storeid, null, null, -1, null, null, -1);

            Client client = new Client(
                reader.GetInt32(reader.GetOrdinal("UserId")),
                reader.GetString(reader.GetOrdinal("username")),
                reader.GetString(reader.GetOrdinal("password")),
                reader.GetString(reader.GetOrdinal("firstname")),
                reader.GetString(reader.GetOrdinal("lastname")),
                reader.GetString(reader.GetOrdinal("phonenumber")),
                reader.GetInt32(reader.GetOrdinal("postalcode")),
                reader.GetString(reader.GetOrdinal("Cityname")),
                reader.GetString(reader.GetOrdinal("streetname")),
                reader.GetInt32(reader.GetOrdinal("housenumber"))
            );

            TimeOnly start = TimeOnly.FromTimeSpan(reader.GetTimeSpan(reader.GetOrdinal("start_")));
            TimeOnly end = TimeOnly.FromTimeSpan(reader.GetTimeSpan(reader.GetOrdinal("end_")));

            Timeslot timeslot = new Timeslot(
                reader.GetInt32(reader.GetOrdinal("TimeslotId")),
                start,
                end,
                store
            );

            Enum.TryParse(reader.GetString(reader.GetOrdinal("status")), true, out OrderStatus status);
            int boxes = reader.GetInt32(reader.GetOrdinal("boxes"));
            DateOnly fetchdate = DateOnly.FromDateTime(reader.GetDateTime(reader.GetOrdinal("fetchdate")));

            return new Order(
                reader.GetInt32(reader.GetOrdinal("OrderId")),
                status,
                boxes,
                client,
                store,
                timeslot,
                null,
                fetchdate
            );
        }

        public async Task<Order?> GetOrderAsync(int id)
        {
            Order? o = null;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(
                    @"SELECT o.status, o.boxes, o.StoreId, o.TimeslotId, o.fetchdate, o.UserId,
                      u.username, u.password, u.firstname, u.lastname,
                      u.phonenumber, u.postalcode, u.Cityname, u.streetname, u.housenumber,
                      t.start_, t.end_
                      FROM dbo.Order_ o
                      JOIN dbo.Timeslot t ON o.TimeslotId = t.TimeslotId
                      JOIN dbo.User_ u ON o.UserId = u.UserId
                      WHERE o.OrderId = @id",
                    conn);
                cmd.Parameters.AddWithValue("id", id);
                await conn.OpenAsync();

                using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        int uid = reader.GetInt32(reader.GetOrdinal("UserId"));
                        Client cl = new Client(
                            uid,
                            reader.GetString(reader.GetOrdinal("username")),
                            reader.GetString(reader.GetOrdinal("password")),
                            reader.GetString(reader.GetOrdinal("firstname")),
                            reader.GetString(reader.GetOrdinal("lastname")),
                            reader.GetString(reader.GetOrdinal("phonenumber")),
                            reader.GetInt32(reader.GetOrdinal("postalcode")),
                            reader.GetString(reader.GetOrdinal("Cityname")),
                            reader.GetString(reader.GetOrdinal("streetname")),
                            reader.GetInt32(reader.GetOrdinal("housenumber"))
                        );
                        int boxes = reader.GetInt32(reader.GetOrdinal("boxes"));
                        Enum.TryParse(reader.GetString(reader.GetOrdinal("status")), true, out OrderStatus status);
                        int storeid = reader.GetInt32(reader.GetOrdinal("StoreId"));
                        int timeslotid = reader.GetInt32(reader.GetOrdinal("TimeslotId"));
                        TimeOnly start = TimeOnly.FromTimeSpan(reader.GetTimeSpan(reader.GetOrdinal("start_")));
                        TimeOnly end = TimeOnly.FromTimeSpan(reader.GetTimeSpan(reader.GetOrdinal("end_")));
                        DateOnly fetchdate = DateOnly.FromDateTime(reader.GetDateTime(reader.GetOrdinal("fetchdate")));
                        Store store = new Store(storeid, null, null, -1, null, null, -1);
                        Timeslot tslot = new Timeslot(timeslotid, start, end, store);
                        o = new Order(id, status, boxes, cl, store, tslot, null, fetchdate);
                        o = await GetOrderlinesAsync(o);

                    }
                }
            }
            return o;

        }

        public async Task<Order> GetOrderlinesAsync(Order o)
        {
            List<OrderLine> orderlines = new List<OrderLine>();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(@"SELECT l.OrderLineId, l.quantity,p.ProductId, p.name, p.price, p.CategoryId,
                     c.Name AS CategoryName FROM dbo.Order_line l JOIN dbo.Product p ON l.ProductId = p.ProductId
                     JOIN dbo.Category c ON p.CategoryId = c.CategoryId WHERE l.OrderId = @id",conn);

                cmd.Parameters.AddWithValue("@id", o.OrderId);
                await conn.OpenAsync();

                using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        int orderLineId = reader.GetInt32(reader.GetOrdinal("OrderLineId"));
                        int quantity = reader.GetInt32(reader.GetOrdinal("quantity"));

                        int productId = reader.GetInt32(reader.GetOrdinal("ProductId"));
                        string productName = reader.GetString(reader.GetOrdinal("name"));
                        decimal price = reader.GetDecimal(reader.GetOrdinal("price"));

                        int categoryId = reader.GetInt32(reader.GetOrdinal("CategoryId"));
                        string categoryName = reader.GetString(reader.GetOrdinal("CategoryName"));

                        Category cat = new Category(categoryId, categoryName);

                        Product product = new Product(productId, productName, price, cat);

                        OrderLine line = new OrderLine(orderLineId, quantity, product, o);

                        orderlines.Add(line);
                    }
                }
            }

            o.Orderlines = orderlines;

            return o;
        }


        public async Task<bool> FinalizeOrderAsync(int orderId)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(
                    "UPDATE dbo.Order_ SET status = 'Finalized' WHERE OrderId = @orderId AND status = 'Prepared'",
                    connection);

                cmd.Parameters.AddWithValue("@orderId", orderId);
                await connection.OpenAsync();

                int result = await cmd.ExecuteNonQueryAsync();
                return result > 0;
            }
        }

        public Task<bool> RemoveOrderAsync(Order o)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> UpdateOrderAsync(Order o, OrderStatus expectedStatus)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("UPDATE dbo.Order_ SET status = @status,boxes = @boxes,"
                    +"StoreId = @storeId,TimeslotId = @timeslotId,UserId = @userId,fetchdate = @fetchdate WHERE OrderId = @orderId AND status = @expectedStatus",conn);

                cmd.Parameters.AddWithValue("@status", o.Status.ToString());
                cmd.Parameters.AddWithValue("@boxes", o.BoxesInvolved);
                cmd.Parameters.AddWithValue("@storeId", o.Store.StoreId);
                cmd.Parameters.AddWithValue("@timeslotId", o.Timeslot.TimeslotId);
                cmd.Parameters.AddWithValue("@userId", o.Client.UserId);
                cmd.Parameters.AddWithValue("@fetchdate", o.Fetchdate);
                cmd.Parameters.AddWithValue("@orderId", o.OrderId);
                cmd.Parameters.AddWithValue("@expectedStatus", expectedStatus.ToString());

                await conn.OpenAsync();

                int rows = await cmd.ExecuteNonQueryAsync();

                return rows > 0;
            }
        }

        public async Task<List<Order>> GetTomorrowOrdersByStoreAsync(int storeId)
        {
            List<Order> orders = new List<Order>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(
                    @"SELECT
                o.OrderId,
                o.status,
                o.boxes,
                o.fetchdate,
                o.TimeslotId,
                o.StoreId,
                u.UserId,
                u.username, u.password, u.firstname, u.lastname,
                u.phonenumber, u.postalcode, u.Cityname, u.streetname, u.housenumber,
                t.start_,
                t.end_
              FROM dbo.Order_ o
              INNER JOIN dbo.User_ u ON o.UserId = u.UserId
              INNER JOIN dbo.Timeslot t ON o.TimeslotId = t.TimeslotId
              WHERE o.StoreId = @storeId
                AND o.fetchdate = DATEADD(day, 1, CAST(GETDATE() AS DATE))
                AND o.status != 'Finalized'
              ORDER BY t.start_",
                    connection);

                cmd.Parameters.AddWithValue("@storeId", storeId);
                await connection.OpenAsync();

                using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        orders.Add(ReadOrderListRow(reader));
                    }
                }
            }

            return orders;
        }
    }
}

