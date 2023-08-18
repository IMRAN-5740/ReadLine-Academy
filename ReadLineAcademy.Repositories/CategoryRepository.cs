using ReadLineAcademy.Databases.Data;
using ReadLineAcademy.Repositories.Absractions;
using ReadLineAcademy.Repositories.Base;
using ReadLineAcademy.Models.EntityModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReadLineAcademy.Repositories
{
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        
        public CategoryRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            _dbContext=dbContext;
        }

      
        public void Update(Category entity)
        {
            dbSet.Update(entity);
        }
    }
}
