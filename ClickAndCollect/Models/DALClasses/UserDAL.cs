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
                    "SELECT COUNT(*) FROM dbo.User_ WHERE UserId = @userId AND Password = @password",
                    connection);

                cmd.Parameters.AddWithValue("@userId", userId);
                cmd.Parameters.AddWithValue("@password", password);
                await connection.OpenAsync();

                int count = (int)await cmd.ExecuteScalarAsync();
                return count > 0;
            }
        }
        public async Task<User?> GetUserByIdAsync(int userId)
        {
            User? user = null;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(
                    "SELECT * FROM dbo.User_ WHERE UserId = @userId",
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
                                user = new Client
                                {
                                    UserId = id,
                                    Username = uname,
                                    Password = pwd,
                                    Firstname = reader.GetString(reader.GetOrdinal("firstname")),
                                    Lastname = reader.GetString(reader.GetOrdinal("lastname")),
                                    Phonenumber = reader.GetString(reader.GetOrdinal("phonenumber")),
                                    Postalcode = reader.GetInt32(reader.GetOrdinal("postalcode")),
                                    CityName = reader.GetString(reader.GetOrdinal("Cityname")),
                                    StreetName = reader.GetString(reader.GetOrdinal("streetname")),
                                    HouseNumber = reader.GetInt32(reader.GetOrdinal("housenumber"))
                                };
                                break;
                            case "Cashier":
                                user = new Cashier
                                {
                                    UserId = id,
                                    Username = uname,
                                    Password = pwd,
                                    StoreId = reader.GetInt32(reader.GetOrdinal("StoreId"))
                                };
                                break;
                            case "OrderMaker":
                                user = new OrderMaker
                                {
                                    UserId = id,
                                    Username = uname,
                                    Password = pwd,
                                    StoreId = reader.GetInt32(reader.GetOrdinal("StoreId"))
                                };
                                break;
                        }
                    }
                }
            }
            return user;
        }
    }
}
