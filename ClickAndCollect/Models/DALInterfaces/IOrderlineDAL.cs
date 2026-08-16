using ClickAndCollect.Models.Classes;

namespace ClickAndCollect.Models.DALInterfaces
{
    public interface IOrderlineDAL
    {
        public abstract Task<bool> AddOrderlinesAsync(List<OrderLine> lines);
    }
}
