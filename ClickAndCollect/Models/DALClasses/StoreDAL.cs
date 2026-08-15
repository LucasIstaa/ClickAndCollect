using ClickAndCollect.Models.Classes;
using ClickAndCollect.Models.DALInterfaces;
using Microsoft.Data.SqlClient;

namespace ClickAndCollect.Models.DALClasses
{
    public class StoreDAL : IStoreDAL
    {
        private String connectionString;

        public StoreDAL(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public async Task<bool> AddStoreAsync(Store s)
        {
            bool success = false;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("INSERT INTO dbo.Store(phonenumber,name,postalcode,cityname,streetname,housenumber)"+
                    "VALUES(@phonenumber,@name,@postalcode,@cityname,@streetname,@housenumber)", conn);

                cmd.Parameters.AddWithValue("phonenumber", s.PhoneNumber);
                cmd.Parameters.AddWithValue("name", s.Name);
                cmd.Parameters.AddWithValue("postalcode", s.PostalCode);
                cmd.Parameters.AddWithValue("cityname", s.CityName);
                cmd.Parameters.AddWithValue("streetname", s.StreetName);
                cmd.Parameters.AddWithValue("housenumber", s.HouseNumber);

                await conn.OpenAsync();

                int res = await cmd.ExecuteNonQueryAsync();

                success = res > 0;
            }

            return success;
        }

        public async Task<List<Store>> GetAllStoresAsync()
        {
            List<Store> list = new List<Store>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("SELECT * FROM dbo.Store", connection);
                await connection.OpenAsync();

                using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        int id = reader.GetInt32(reader.GetOrdinal("StoreId"));
                        string pn = reader.GetString(reader.GetOrdinal("phonenumber"));
                        string name = reader.GetString(reader.GetOrdinal("name"));
                        int postal = reader.GetInt32(reader.GetOrdinal("postalcode"));
                        string cname = reader.GetString(reader.GetOrdinal("cityname"));
                        string sname = reader.GetString(reader.GetOrdinal("streetname"));
                        int hnumber = reader.GetInt32(reader.GetOrdinal("housenumber"));
                        Store s = new Store(id,pn,name,postal,cname,sname,hnumber);
                        list.Add(s);
                    }
                }

            }
            return list;
        }

        public async Task<Store> GetStoreAsync(int id)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("SELECT * FROM dbo.Store WHERE StoreId = @id",connection);

                cmd.Parameters.AddWithValue("@id", id);

                await connection.OpenAsync();

                using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        int storeId = reader.GetInt32(reader.GetOrdinal("StoreId"));
                        string pn = reader.GetString(reader.GetOrdinal("phonenumber"));
                        string name = reader.GetString(reader.GetOrdinal("name"));
                        int postal = reader.GetInt32(reader.GetOrdinal("postalcode"));
                        string cname = reader.GetString(reader.GetOrdinal("cityname"));
                        string sname = reader.GetString(reader.GetOrdinal("streetname"));
                        int hnumber = reader.GetInt32(reader.GetOrdinal("housenumber"));

                        return new Store(storeId, pn, name, postal, cname, sname, hnumber);
                    }
                }
            }

            return null;
        }


        public Task<List<Timeslot>> GetStoreTimeslotsAsync(int storeId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> RemoveStoreAsync(Store s)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateStoreAsync(Store s)
        {
            throw new NotImplementedException();
        }
    }
}
