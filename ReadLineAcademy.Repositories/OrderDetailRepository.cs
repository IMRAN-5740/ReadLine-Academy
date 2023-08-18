using ReadLineAcademy.Databases.Data;
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
    public class OrderDetailRepository : Repository<OrderDetail>, IOrderDetailRepository
    {
        
        public OrderDetailRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            _dbContext=dbContext;
        }

      
        public void Update(OrderDetail entity)
        {
            dbSet.Update(entity);
        }
    }
}
