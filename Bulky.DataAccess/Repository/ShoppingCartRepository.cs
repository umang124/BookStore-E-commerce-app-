using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models.Models;

namespace Bulky.DataAccess.Repository
{
    public class ShoppingCartRepository : Repository<ShoppingCart>, IShoppingCartRepository
    {
        private readonly ApplicationDbContext _db;
        public ShoppingCartRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(ShoppingCart shopping)
        {
            _db.ShoppingCarts.Update(shopping);
        }
    }
}
