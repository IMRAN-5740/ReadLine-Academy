using ReadLineAcademy.Databases.Data;
using ReadLineAcademy.Databases.Migrations;
using ReadLineAcademy.Models.EntityModels;
using ReadLineAcademy.Repositories.Absractions;
using ReadLineAcademy.Repositories.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReadLineAcademy.Repositories
{
    public class ShoppingCartRepository : Repository<ShoppingCart>, IShoppingCartRepository
    {
        public ShoppingCartRepository(ApplicationDbContext dbContext) :base(dbContext)
        {
            _dbContext = dbContext;
        }

        public int DecrementCount(ShoppingCart shoppingCart, int count)
        {
           shoppingCart.Count -= count;  
            return shoppingCart.Count;
        }

        public int IncrementCount(ShoppingCart shoppingCart, int count)
        {
            shoppingCart.Count += count;
            return shoppingCart.Count;
        }
    }
}
