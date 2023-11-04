using Bulky.Models.Models;

namespace Bulky.DataAccess.Repository.IRepository
{
    public interface IShoppingCartRepository : IRepository<ShoppingCart>
    {
        void Update(ShoppingCart shopping);
    }
}
