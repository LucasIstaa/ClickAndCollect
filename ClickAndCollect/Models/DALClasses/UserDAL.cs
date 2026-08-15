using ClickAndCollect.Models.Classes;
using ClickAndCollect.Models.DALInterfaces;
using Microsoft.Data.SqlClient;

namespace ClickAndCollect.Models.DALClasses
{
    public class UserDAL : IUserDAL
    {
        private string connectionString;

        public UserDAL(string conn)
        {
            this.connectionString = conn;
        }

        public async Task<int?> GetUserIdByUsernameAsync(string username)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(
                    "SELECT UserId FROM dbo.User_ WHERE username = @username",
                    connection);

                cmd.Parameters.AddWithValue("@username", username);
                await connection.OpenAsync();

                object? result = await cmd.ExecuteScalarAsync();

                if (result != null)
                    return (int)result;

                return null;
            }
        }

        public async Task<bool> VerifyPasswordAsync(int userId, string password)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(
                    "SELECT Password FROM dbo.User_ WHERE UserId = @userId",
                    connection);

                cmd.Parameters.AddWithValue("@userId", userId);
                await connection.OpenAsync();

                object? result = await cmd.ExecuteScalarAsync();
                if (result == null)
                    return false;

                string storedHash = (string)result;
                return BCrypt.Net.BCrypt.Verify(password, storedHash);
            }
        }

        public async Task<User?> GetUserByIdAsync(int userId)
        {
            User? user = null;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(
                    @"SELECT u.*, s.StoreId AS SId, s.phonenumber AS SPhone, s.name AS SName, 
              s.postalcode AS SPostal, s.cityname AS SCity, 
              s.streetname AS SStreet, s.housenumber AS SHouse
              FROM dbo.User_ u
              LEFT JOIN dbo.Store s ON u.StoreId = s.StoreId
              WHERE u.UserId = @userId",
                    connection);

                cmd.Parameters.AddWithValue("@userId", userId);
                await connection.OpenAsync();

                using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        int id = reader.GetInt32(reader.GetOrdinal("UserId"));
                        string uname = reader.GetString(reader.GetOrdinal("username"));
                        string pwd = reader.GetString(reader.GetOrdinal("password"));
                        string role = reader.GetString(reader.GetOrdinal("role"));

                        switch (role)
                        {
                            case "Client":
                                user = new Client(
                                    id, uname, pwd,
                                    reader.GetString(reader.GetOrdinal("firstname")),
                                    reader.GetString(reader.GetOrdinal("lastname")),
                                    reader.GetString(reader.GetOrdinal("phonenumber")),
                                    reader.GetInt32(reader.GetOrdinal("postalcode")),
                                    reader.GetString(reader.GetOrdinal("Cityname")),
                                    reader.GetString(reader.GetOrdinal("streetname")),
                                    reader.GetInt32(reader.GetOrdinal("housenumber"))
                                );
                                break;
                            case "Cashier":
                                Store cashierStore = new Store(
                                    reader.GetInt32(reader.GetOrdinal("SId")),
                                    reader.GetString(reader.GetOrdinal("SPhone")),
                                    reader.GetString(reader.GetOrdinal("SName")),
                                    reader.GetInt32(reader.GetOrdinal("SPostal")),
                                    reader.GetString(reader.GetOrdinal("SCity")),
                                    reader.GetString(reader.GetOrdinal("SStreet")),
                                    reader.GetInt32(reader.GetOrdinal("SHouse"))
                                );
                                user = new Cashier(id, uname, pwd, cashierStore);
                                break;
                            case "OrderMaker":
                                Store makerStore = new Store(
                                    reader.GetInt32(reader.GetOrdinal("SId")),
                                    reader.GetString(reader.GetOrdinal("SPhone")),
                                    reader.GetString(reader.GetOrdinal("SName")),
                                    reader.GetInt32(reader.GetOrdinal("SPostal")),
                                    reader.GetString(reader.GetOrdinal("SCity")),
                                    reader.GetString(reader.GetOrdinal("SStreet")),
                                    reader.GetInt32(reader.GetOrdinal("SHouse"))
                                );
                                user = new OrderMaker(id, uname, pwd, makerStore);
                                break;
                        }
                    }
                }
            }
            return user;
        }

        public async Task<bool> CreateAccountAsync(Client client)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(
                    @"INSERT INTO dbo.User_ (username, password, firstname, lastname, phonenumber, 
              postalcode, Cityname, streetname, housenumber, role)
              VALUES (@username, @password, @firstname, @lastname, @phonenumber, 
              @postalcode, @cityname, @streetname, @housenumber, 'Client')",
                    connection);

                cmd.Parameters.AddWithValue("@username", client.Username);
                cmd.Parameters.AddWithValue("@password", BCrypt.Net.BCrypt.HashPassword(client.Password));
                cmd.Parameters.AddWithValue("@firstname", client.Firstname);
                cmd.Parameters.AddWithValue("@lastname", client.Lastname);
                cmd.Parameters.AddWithValue("@phonenumber", client.Phonenumber);
                cmd.Parameters.AddWithValue("@postalcode", client.Postalcode);
                cmd.Parameters.AddWithValue("@cityname", client.CityName);
                cmd.Parameters.AddWithValue("@streetname", client.StreetName);
                cmd.Parameters.AddWithValue("@housenumber", client.HouseNumber);

                await connection.OpenAsync();

                int result = await cmd.ExecuteNonQueryAsync();
                return result > 0;
            }
        }
    }
}