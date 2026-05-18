
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
                cmd.Parameters.AddWithValue("stat", o.Status);
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

                if (await AddOrderlinesAsync(o.Orderlines)) 
                {
                    success = o.OrderId > 0;
                }
            }
            return success;
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
                        Store store = new Store(storeid);
                        Timeslot tslot = new Timeslot(timeslotid, start, end, store);
                        List<OrderLine> lines = await GetOrderlinesAsync(orderid);
                        Order o = new Order(orderid, status, boxes, cl, store, tslot, lines, fetchdate);

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
                    @"SELECT o.OrderId, o.status, o.boxes,
                      u.UserId, u.username, u.password, u.firstname, u.lastname,
                      u.phonenumber, u.postalcode, u.Cityname, u.streetname, u.housenumber,
                      t.TimeslotId, t.start_, t.end_,
                      s.StoreId, s.phonenumber AS SPhone, s.name AS SName,
                      s.postalcode AS SPostal, s.cityname AS SCity,
                      s.streetname AS SStreet, s.housenumber AS SHouse
                      FROM dbo.Order_ o
                      INNER JOIN dbo.User_ u ON o.UserId = u.UserId
                      INNER JOIN dbo.Timeslot t ON o.TimeslotId = t.TimeslotId
                      INNER JOIN dbo.Store s ON o.StoreId = s.StoreId
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
                        Store store = new Store(
                            reader.GetInt32(reader.GetOrdinal("StoreId")),
                            reader.GetString(reader.GetOrdinal("SPhone")),
                            reader.GetString(reader.GetOrdinal("SName")),
                            reader.GetInt32(reader.GetOrdinal("SPostal")),
                            reader.GetString(reader.GetOrdinal("SCity")),
                            reader.GetString(reader.GetOrdinal("SStreet")),
                            reader.GetInt32(reader.GetOrdinal("SHouse"))
                        );

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

                        Timeslot timeslot = new Timeslot(
                            reader.GetInt32(reader.GetOrdinal("TimeslotId")),
                            TimeOnly.FromDateTime(reader.GetDateTime(reader.GetOrdinal("start_"))),
                            TimeOnly.FromDateTime(reader.GetDateTime(reader.GetOrdinal("end_"))),
                            store
                        );

                        string statusStr = reader.GetString(reader.GetOrdinal("status"));
                        OrderStatus orderStatus = Enum.Parse<OrderStatus>(statusStr);

                        /*Order order = new Order(
                            reader.GetInt32(reader.GetOrdinal("OrderId")),
                            orderStatus,
                            reader.GetInt32(reader.GetOrdinal("boxes")),
                            client,
                            store,
                            timeslot,
                            null
                        );*/

                        //orders.Add(order);
                    }
                }
            }
            return orders;
        }

        public async Task<Order> GetOrderAsync(int id)
        {
            Order o = null;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(
                    @"SELECT o.status, o.boxes, o.StoreId, o.TimeslotId, o.fetchdate,
                      t.start_, t.end_
                      FROM dbo.Order_ o
                      JOIN dbo.Timeslot t ON o.TimeslotId = t.TimeslotId
                      WHERE o.OrderId = @id",
                    conn);
                cmd.Parameters.AddWithValue("id", id);
                await conn.OpenAsync();

                using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        Client cl = new Client(id, null, null, null, null, null, 0, null, null, 0);
                        int boxes = reader.GetInt32(reader.GetOrdinal("boxes"));
                        Enum.TryParse(reader.GetString(reader.GetOrdinal("status")), true, out OrderStatus status);
                        int storeid = reader.GetInt32(reader.GetOrdinal("StoreId"));
                        int timeslotid = reader.GetInt32(reader.GetOrdinal("TimeslotId"));
                        TimeOnly start = TimeOnly.FromTimeSpan(reader.GetTimeSpan(reader.GetOrdinal("start_")));
                        TimeOnly end = TimeOnly.FromTimeSpan(reader.GetTimeSpan(reader.GetOrdinal("end_")));
                        DateOnly fetchdate = DateOnly.FromDateTime(reader.GetDateTime(reader.GetOrdinal("fetchdate")));
                        Store store = new Store(storeid);
                        Timeslot tslot = new Timeslot(timeslotid, start, end, store);
                        List<OrderLine> lines = await GetOrderlinesAsync(id);
                        o = new Order(id, status, boxes, cl, store, tslot, lines, fetchdate);

                    }
                }
            }
            return o;

        }

        public async Task<Order?> GetOrderByIdAsync(int orderId)
        {
            Order? order = null;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(
                    @"SELECT o.OrderId, o.status, o.boxes,
                      u.UserId, u.username, u.password, u.firstname, u.lastname,
                      u.phonenumber, u.postalcode, u.Cityname, u.streetname, u.housenumber,
                      t.TimeslotId, t.start_, t.end_,
                      s.StoreId, s.phonenumber AS SPhone, s.name AS SName,
                      s.postalcode AS SPostal, s.cityname AS SCity,
                      s.streetname AS SStreet, s.housenumber AS SHouse
                      FROM dbo.Order_ o
                      INNER JOIN dbo.User_ u ON o.UserId = u.UserId
                      INNER JOIN dbo.Timeslot t ON o.TimeslotId = t.TimeslotId
                      INNER JOIN dbo.Store s ON o.StoreId = s.StoreId
                      WHERE o.OrderId = @orderId",
                    connection);

                cmd.Parameters.AddWithValue("@orderId", orderId);
                await connection.OpenAsync();

                using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        Store store = new Store(
                            reader.GetInt32(reader.GetOrdinal("StoreId")),
                            reader.GetString(reader.GetOrdinal("SPhone")),
                            reader.GetString(reader.GetOrdinal("SName")),
                            reader.GetInt32(reader.GetOrdinal("SPostal")),
                            reader.GetString(reader.GetOrdinal("SCity")),
                            reader.GetString(reader.GetOrdinal("SStreet")),
                            reader.GetInt32(reader.GetOrdinal("SHouse"))
                        );

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

                        Timeslot timeslot = new Timeslot(
                            reader.GetInt32(reader.GetOrdinal("TimeslotId")),
                            TimeOnly.FromDateTime(reader.GetDateTime(reader.GetOrdinal("start_"))),
                            TimeOnly.FromDateTime(reader.GetDateTime(reader.GetOrdinal("end_"))),
                            store
                        );

                        string statusStr = reader.GetString(reader.GetOrdinal("status"));
                        OrderStatus orderStatus = Enum.Parse<OrderStatus>(statusStr);

                        /*order = new Order(
                            reader.GetInt32(reader.GetOrdinal("OrderId")),
                            orderStatus,
                            reader.GetInt32(reader.GetOrdinal("boxes")),
                            client,
                            store,
                            timeslot
                        );*/
                    }
                }
            }
            return order;
        }

        public async Task<List<OrderLine>> GetOrderlinesAsync(int id)
        {
            List<OrderLine> orderlines = new List<OrderLine>();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(
                    @"SELECT l.OrderLineId, l.quantity, p.name, p.price, p.ProductId, p.CategoryId
                      FROM dbo.Order_line l
                      JOIN dbo.Product p ON l.ProductId = p.ProductId
                      WHERE l.OrderId = @id",
                    conn);
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

                        Product p = new Product(pid, pname, price, new Category(catid, null));
                        OrderLine ol = new OrderLine(ordlid, quantity, p, null);

                        orderlines.Add(ol);
                    }
                }
            }

            return orderlines;
        }

        public async Task<bool> SetBoxesReturnedAsync(int orderId, int nbBoxes)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(
                    "UPDATE dbo.Order_ SET boxesReturned = @nbBoxes WHERE OrderId = @orderId",
                    connection);

                cmd.Parameters.AddWithValue("@nbBoxes", nbBoxes);
                cmd.Parameters.AddWithValue("@orderId", orderId);
                await connection.OpenAsync();

                int result = await cmd.ExecuteNonQueryAsync();
                return result > 0;
            }
        }

        public async Task<decimal> CalculateFinalPriceAsync(int orderId, int boxesReturned)
        {
            decimal totalProducts = 0;
            int boxesUsed = 0;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(
                    @"SELECT o.boxes,
                      ISNULL(SUM(ol.quantity * p.price), 0) AS TotalProducts
                      FROM dbo.Order_ o
                      LEFT JOIN dbo.Order_line ol ON o.OrderId = ol.OrderId
                      LEFT JOIN dbo.Product p ON ol.ProductId = p.ProductId
                      WHERE o.OrderId = @orderId
                      GROUP BY o.boxes",
                    connection);

                cmd.Parameters.AddWithValue("@orderId", orderId);
                await connection.OpenAsync();

                using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        boxesUsed = reader.GetInt32(reader.GetOrdinal("boxes"));
                        totalProducts = reader.GetDecimal(reader.GetOrdinal("TotalProducts"));
                    }
                }
            }

            decimal serviceFee = 5.95m;
            decimal boxFee = 5.95m * boxesUsed;
            decimal boxRefund = 5.95m * boxesReturned;

            return totalProducts + serviceFee + boxFee - boxRefund;
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

        public Task<bool> RemoveOrderAsync(Order o)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateOrderAsync(Order o)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> AddOrderlinesAsync(List<OrderLine> lines)
        {
            bool success = true;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                await conn.OpenAsync();

                foreach (OrderLine line in lines)
                {
                    SqlCommand cmd = new SqlCommand(
                        "INSERT INTO Order_line (OrderId, ProductId, Quantity) " +
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

