using ReadLineAcademy.Databases.Data;
using ReadLineAcademy.Repositories.Absractions.Base;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace ReadLineAcademy.Repositories.Base
{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected ApplicationDbContext _dbContext;
        internal DbSet<T> dbSet;
        public Repository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
            //_dbContext.ShoppingCarts.AsNoTracking();
                //  _dbContext.ShoppingCarts.Include(u => u.Product);
            this.dbSet = _dbContext.Set<T>();

        }
        public void Add(T entity)
        {
            dbSet.Add(entity);
        }
        //Include Category,CoverType

        public ICollection<T> GetAll(Expression<Func<T, bool>>? predicate=null, string? includeProperties = null)
        {
            IQueryable<T> query = dbSet;
           
            if(predicate!=null)
            {
                query = query.Where(predicate);
            }
            if (includeProperties != null)
            {
                foreach (var property in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(property);
                }
            }
            return query.ToList();
        }

        public T GetFirstOrDefault(Expression<Func<T, bool>> predicate, string? includeProperties = null, bool tracked = true)
        {
            IQueryable<T> query;
            if(tracked)
            {
               query= dbSet;
            }
            else
            {
                query = dbSet.AsNoTracking();
            }
            query = query.Where(predicate);

            if (includeProperties != null)
            {
                foreach (var property in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(property);
                }
            }
            return query.FirstOrDefault(predicate);
        }

        public void Remove(T entity)
        {
            dbSet.Remove(entity);
        }

        public void RemoveRange(IEnumerable<T> entity)
        {
            dbSet.RemoveRange(entity);
        }
    }
}
