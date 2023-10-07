using Bulky.DataAccess.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.DataAccess.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private ApplicationDbContext _db;
        public ICategoryRepository CategoryRepository { get; private set; }
        public UnitOfWork(ApplicationDbContext db)
        {
            _db = db;
            CategoryRepository = new CategoryRepository(_db);
        }
        public void Save()
        {
            _db.SaveChanges();
        }
    }
}
