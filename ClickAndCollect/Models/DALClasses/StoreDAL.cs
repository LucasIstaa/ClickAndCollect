using ClickAndCollect.Models.DALInterfaces;
using Microsoft.Data.SqlClient;

namespace ClickAndCollect.Models.DAL
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

        public Task<List<Store>> GetAllStoresAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Store> GetStoreAsync(int id)
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
