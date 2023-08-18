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
    public class OrderHeaderRepository : Repository<OrderHeader>, IOrderHeaderRepository
    {
        
        public OrderHeaderRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            _dbContext=dbContext;
        }

      
        public void Update(OrderHeader entity)
        {
            dbSet.Update(entity);
        }

        public void UpdateStatus(int id, string orderStatus, string? paymentStatus = null)
        {
            var orderFromDb = _dbContext.OrderHeaders.FirstOrDefault(data => data.Id == id);
            if(orderFromDb != null)
            {
                orderFromDb.OrderStatus=orderStatus;
                if(paymentStatus!=null)
                {
                    orderFromDb.PaymentStatus=paymentStatus;
                }
            }
        





        }
        public void UpdateStripePayment(int id, string sessionId, string paymentIntendId)
        {
            var orderFromDb = _dbContext.OrderHeaders.FirstOrDefault(data => data.Id == id);
            orderFromDb.PaymentDate=DateTime.Now;
            orderFromDb.SessionId = sessionId;
            orderFromDb.PaymentIntentId=paymentIntendId;
        }
    }
}
