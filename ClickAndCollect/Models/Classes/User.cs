using ClickAndCollect.Models.DALInterfaces;

namespace ClickAndCollect.Models.Classes
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

        protected User(string username, string password)
        {
            Username = username;
            Password = password;
        }

        protected User(int userId, string username, string password) : this(username, password) 
        {
            UserId = userId;
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
        public static async Task<User?> GetUser(int userId, IUserDAL dal)
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

        public static List<string> ValidateRegistrationData(string password, string firstname, string lastname)
        {
            List<string> errors = new List<string>();

            if (password.Length < 12 || password.Length > 64)
                errors.Add("Password must be between 12 and 64 characters.");
            if (!password.Any(char.IsLetter) || !password.Any(char.IsDigit))
                errors.Add("Password must contain at least one letter and one number.");
            if (firstname.Length < 3 || firstname.Length > 255 || firstname.Any(char.IsDigit))
                errors.Add("First name must be 3-255 characters with no digits.");
            if (lastname.Length < 3 || lastname.Length > 255 || lastname.Any(char.IsDigit))
                errors.Add("Last name must be 3-255 characters with no digits.");

            return errors;
        }

        public static async Task<bool> CreateAccount(string username, string password, string firstname, string lastname, string phonenumber, int postalcode, string cityname, string streetname, int housenumber, IUserDAL dal)
        {
            Client c = new Client(username, password, firstname, lastname, phonenumber, postalcode, cityname, streetname, housenumber);
            bool saved = await dal.CreateAccountAsync(c);
            return saved;
        }
    }
}
