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
    public class CompanyRepository : Repository<Company>, ICompanyRepository
    {
        public CompanyRepository(ApplicationDbContext dbContext):base(dbContext)
        {
           _dbContext = dbContext;
        }
        public void Update(Company entity)
        {
            var existingProduct = dbSet.FirstOrDefault(x => x.Id == entity.Id);
            if (existingProduct != null)
            {
                existingProduct.Name = entity.Name;
                existingProduct.StreetAddress = entity.StreetAddress;
                existingProduct.City = entity.City;
                existingProduct.State = entity.State;
                existingProduct.PostalCode = entity.PostalCode;
                existingProduct.PhoneNumber = entity.PhoneNumber;
            }
           

            dbSet.Update(existingProduct); 
        }
    }
}
