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
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        public ProductRepository(ApplicationDbContext dbContext):base(dbContext)
        {
           _dbContext = dbContext;
        }
        public void Update(Product entity)
        {
            var existingProduct = dbSet.FirstOrDefault(x => x.Id == entity.Id);
            if (existingProduct != null)
            {
                existingProduct.Title = entity.Title;
                existingProduct.Description = entity.Description;
                existingProduct.ISBN = entity.ISBN;
                existingProduct.AuthorName = entity.AuthorName;
                existingProduct.ListPrice = entity.ListPrice;
                existingProduct.Price = entity.Price;
                existingProduct.Price50 = entity.Price50;
                existingProduct.Price100 = entity.Price100;
                
                existingProduct.CategoryId = entity.CategoryId;
                existingProduct.CoverTypeId = entity.CoverTypeId;
                if(entity.ImageURL!=null)
                {
                    existingProduct.ImageURL = entity.ImageURL;
                }
            }
           

            dbSet.Update(existingProduct); 
        }
    }
}
