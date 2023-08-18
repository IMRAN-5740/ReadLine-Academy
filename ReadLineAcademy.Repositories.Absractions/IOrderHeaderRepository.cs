using ReadLineAcademy.Models.EntityModels;
using ReadLineAcademy.Repositories.Absractions.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReadLineAcademy.Repositories.Absractions
{
    public interface IOrderHeaderRepository : IRepository<OrderHeader>
    {
        void Update(OrderHeader entity);
        void UpdateStatus(int id, string orderStatus, string? paymentStatus = null);
        void UpdateStripePayment(int id, string sessionId, string paymentIntendId);

    }
}
