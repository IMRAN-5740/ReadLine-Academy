using ReadLineAcademy.Models.EntityModels;
using ReadLineAcademy.Repositories.Absractions.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReadLineAcademy.Repositories.Absractions
{
    public interface IOrderDetailRepository : IRepository<OrderDetail>
    {
        void Update(OrderDetail entity);
    }
}
