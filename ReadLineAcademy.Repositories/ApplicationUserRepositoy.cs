using ReadLineAcademy.Databases.Data;
using ReadLineAcademy.Databases.Migrations;
using ReadLineAcademy.Models.EntityModels;
using ReadLineAcademy.Repositories.Absractions;
using ReadLineAcademy.Repositories.Base;
using ReadLineAcademy.Models.AuthModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReadLineAcademy.Repositories
{
    public class ApplicationUserRepository : Repository<ApplicationUser>, IApplicationUserRepository
    {
        public ApplicationUserRepository(ApplicationDbContext dbContext) :base(dbContext)
        {
            _dbContext = dbContext;
        }
      
    }
}
