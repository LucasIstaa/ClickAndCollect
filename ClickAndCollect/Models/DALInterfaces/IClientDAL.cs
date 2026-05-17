using ClickAndCollect.Models.Classes;

namespace ClickAndCollect.Models.DALInterfaces
{
    public interface IClientDAL
    {
        Task<bool> AddClientAsync(Client c);
        Task<bool> UpdateClientAsync(Client c);
        Task<bool> RemoveClientAsync(Client c);
        Task<Client> GetClientAsync(int id);
        Task<List<Client>> GetAllClientsAsync();
    }
}

