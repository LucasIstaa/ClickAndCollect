namespace ClickAndCollect.Models.DALInterfaces
{
    public interface IUserDAL
    {
        public Task<int?> GetUserIdByUsernameAsync(string username);
        public Task<bool> VerifyPasswordAsync(int userId, string password);
        public Task<User> GetUserByIdAsync(int userId);
        public Task<bool> CreateAccountAsync(Client client);
    }
}
