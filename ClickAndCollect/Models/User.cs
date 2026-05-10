using ClickAndCollect.Models.DALInterfaces;

namespace ClickAndCollect.Models
{
    public abstract class User
    {
        private int userId;
        private string username;
        private string password;

        public int UserId
        {
            get { return userId; }
            set { userId = value; }
        }

        public string Username
        {
            get { return username; }
            set { username = value; }
        }

        public string Password
        {
            get { return password; }
            set { password = value; }
        }

        public static async Task<int?> GetByUsername(string username, IUserDAL dal)
        {
            int? userId = await dal.GetUserIdByUsernameAsync(username);
            return userId;
        }
        public static async Task<bool> VerifyPassword(string password, int userId, IUserDAL dal)
        {
            bool valid = await dal.VerifyPasswordAsync(userId, password);
            return valid;
        }
        public static async Task<User> GetUser(int userId, IUserDAL dal)
        {
            User? user = await dal.GetUserByIdAsync(userId);
            return user;
        }
        public string GetRole()
        {
            return this switch
            {
                Client => "Client",
                Cashier => "Cashier",
                OrderMaker => "OrderMaker",
                _ => "Unknown"
            };
        }
    }
}
