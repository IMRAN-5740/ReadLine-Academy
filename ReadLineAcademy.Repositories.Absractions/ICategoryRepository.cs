using ReadLineAcademy.Repositories.Absractions.Base;
using ReadLineAcademy.Models.EntityModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReadLineAcademy.Repositories.Absractions
{
    public interface ICategoryRepository : IRepository<Category>
    {
        void Update(Category entity);
    }
}
