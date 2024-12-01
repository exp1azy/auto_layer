using Microsoft.EntityFrameworkCore;

namespace AutoLayer
{
    public class Repository<TEntity>(DbContext dbContext) : IRepository<TEntity> where TEntity : class
    {
        private readonly DbContext _dbContext = dbContext;

        public TEntity GetById(int id) =>
            _dbContext.Set<TEntity>().Find(id) ?? throw new Exception();

        public IEnumerable<TEntity> GetAll() => 
            _dbContext.Set<TEntity>().ToList() ?? throw new Exception();

        public IEnumerable<TEntity> Find(Func<TEntity, bool> predicate) => 
            _dbContext.Set<TEntity>().Where(predicate).ToList() ?? throw new Exception();

        public IEnumerable<TEntity> GetPaged(int pageNumber, int pageSize, Func<TEntity, bool>? orderBy = null, bool isAscending = true)
        {
            var entities = _dbContext.Set<TEntity>().AsQueryable();
            if (orderBy != null)
            {
                return isAscending ? 
                    [.. entities.OrderBy(orderBy).Skip((pageNumber - 1) * pageSize).Take(pageSize)] : 
                    [.. entities.OrderByDescending(orderBy).Skip((pageNumber - 1) * pageSize).Take(pageSize)];
            }
            
            return isAscending ?
                [.. entities.Skip((pageNumber - 1) * pageSize).Take(pageSize)] :
                [.. entities.OrderByDescending(x => x).Skip((pageNumber - 1) * pageSize).Take(pageSize)];
        }

        public void Add(TEntity entityToAdd)
        {
            _dbContext.Set<TEntity>().Add(entityToAdd);
            _dbContext.SaveChanges();
        }

        public void AddRange(IEnumerable<TEntity> entitiesToAdd)
        {
            _dbContext.Set<TEntity>().AddRange(entitiesToAdd);
            _dbContext.SaveChanges();
        }

        public void ExecuteTransaction(Action action)
        {
            using var trans = _dbContext.Database.BeginTransaction();
            try
            {
                action();
                _dbContext.SaveChanges();
                trans.Commit();
            }
            catch (Exception)
            {
                trans.Rollback();
                throw;
            }
        }

        public void Update(TEntity updatedEntity)
        {
            var entityToUpdate = _dbContext.Set<TEntity>().Find(updatedEntity) ?? throw new Exception();
            _dbContext.Entry(entityToUpdate).CurrentValues.SetValues(updatedEntity);
            _dbContext.SaveChanges();
        }

        public void Remove(TEntity entityToRemove)
        {
            _dbContext.Set<TEntity>().Remove(entityToRemove);
            _dbContext.SaveChanges();
        }

        public void RemoveRange(IEnumerable<TEntity> entitiesToRemove)
        {
            _dbContext.Set<TEntity>().RemoveRange(entitiesToRemove);
            _dbContext.SaveChanges();
        }
    }
}
