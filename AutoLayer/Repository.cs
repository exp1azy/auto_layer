using AutoLayer.Exceptions;
using AutoLayer.Resources;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace AutoLayer
{
    public class Repository<TEntity>(DbContext dbContext) : IRepository<TEntity> where TEntity : class, new()
    {
        private readonly DbContext _dbContext = dbContext;

        #region Sync
        public TEntity? GetById(int id)
        {
            if (id <= 0)
                throw new NonPositiveIdException(Error.NonPositiveIdError);
            
            return _dbContext.Set<TEntity>().Find(id);
        }

        public IEnumerable<TEntity> GetAll(bool asNoTracking = true)
        {
            return asNoTracking ?
                [.. _dbContext.Set<TEntity>().AsNoTracking()] :
                [.. _dbContext.Set<TEntity>()];
        }

        public IEnumerable<TEntity> GetWhere(Func<TEntity, bool> predicate, bool asNoTracking = true)
        {
            return asNoTracking ?
                [.. _dbContext.Set<TEntity>().AsNoTracking().Where(predicate)] :
                [.. _dbContext.Set<TEntity>().Where(predicate)];
        }

        public bool Exists(Func<TEntity, bool> predicate)
        {
            return _dbContext.Set<TEntity>().Any(predicate);
        }

        public int Count()
        {
            return _dbContext.Set<TEntity>().Count();
        }

        public int CountWhere(Func<TEntity, bool> predicate)
        {
            return _dbContext.Set<TEntity>().Count(predicate);
        }

        public IQueryable<TEntity> GetQuery()
        {
            return _dbContext.Set<TEntity>().AsQueryable();
        }

        public IEnumerable<TEntity> GetOrdered(Func<TEntity, object> orderBy, bool isAscending = true, bool asNoTracking = true)
        {
            var entities = _dbContext.Set<TEntity>().AsQueryable();

            if (asNoTracking)
                entities = entities.AsNoTracking();

            return isAscending ?
                [.. entities.OrderBy(orderBy)] :
                [.. entities.OrderByDescending(orderBy)];
        }

        public IEnumerable<TEntity> GetPaged(int pageNumber, int pageSize, Func<TEntity, object>? orderBy = null, bool isAscending = true, bool asNoTracking = true)
        {
            if (pageNumber <= 0 || pageSize <= 0)
                throw new GetPageException(Error.GetPageError);

            var entities = _dbContext.Set<TEntity>().AsQueryable();

            if (asNoTracking)
                entities = entities.AsNoTracking();

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

        public IQueryable<TEntity> Include(Expression<Func<TEntity, object>> navigationPropertyPath)
        {
            return _dbContext.Set<TEntity>().Include(navigationPropertyPath);
        }

        public void Add(TEntity entityToAdd)
        {
            if (entityToAdd == null)
                throw new NullEntityException(Error.NullEntityError, nameof(entityToAdd));

            _dbContext.Set<TEntity>().Add(entityToAdd);
            _dbContext.SaveChanges();
        }

        public void AddRange(IEnumerable<TEntity> entitiesToAdd)
        {
            if (entitiesToAdd == null || entitiesToAdd.Any(x => x == null))
                throw new NullEntityInCollectionException(Error.NullEntityInCollectionError, nameof(entitiesToAdd));

            _dbContext.Set<TEntity>().AddRange(entitiesToAdd);
            _dbContext.SaveChanges();
        }

        public void Update(TEntity entityToUpdate)
        {
            if (entityToUpdate == null)
                throw new NullEntityException(Error.NullEntityError, nameof(entityToUpdate));

            var keyValues = _dbContext.Model.FindEntityType(typeof(TEntity))?
                .FindPrimaryKey()?
                .Properties
                .Select(p => p.PropertyInfo?.GetValue(entityToUpdate))
                .ToArray();

            if (keyValues == null || keyValues.Any(k => k == null))
                throw new NullPrimaryKeyException(Error.NullPrimaryKeyError, nameof(entityToUpdate));

            var entity = _dbContext.Set<TEntity>().Find(keyValues)
                ?? throw new EntityNotFoundException(Error.EntityNotFoundError, nameof(entityToUpdate));

            _dbContext.Entry(entity).CurrentValues.SetValues(entityToUpdate);
            _dbContext.SaveChanges();
        }

        public void UpdateById(int id, Action<TEntity> updateAction)
        {
            if (id <= 0)
                throw new NonPositiveIdException(Error.NonPositiveIdError);

            var entityToUpdate = _dbContext.Set<TEntity>().Find(id);
            if (entityToUpdate == null)
                throw new EntityNotFoundException(Error.EntityNotFoundError, nameof(entityToUpdate));

            updateAction(entityToUpdate);
            _dbContext.SaveChanges();
        }

        public void UpdateRange(IEnumerable<TEntity> entitiesToUpdate)
        {
            if (entitiesToUpdate == null || !entitiesToUpdate.Any())
                throw new NullEntityInCollectionException(Error.NullEntityInCollectionError, nameof(entitiesToUpdate));

            if (entitiesToUpdate.Any(x => x == null))
                throw new NullEntityInCollectionException(Error.NullEntityInCollectionError, nameof(entitiesToUpdate));

            var primaryKeyProperties = _dbContext.Model.FindEntityType(typeof(TEntity))?
                .FindPrimaryKey()?
                .Properties
                .Select(p => p.PropertyInfo)
                .ToArray();

            if (primaryKeyProperties == null || primaryKeyProperties.Length == 0)
                throw new InvalidOperationException($"Entity {typeof(TEntity).Name} does not have a primary key defined.");

            foreach (var entity in entitiesToUpdate)
            {
                var keyValues = primaryKeyProperties
                    .Select(p => p.GetValue(entity))
                    .ToArray();

                if (keyValues.Any(k => k == null))
                    throw new ArgumentException("Primary key values cannot be null", nameof(entitiesToUpdate));

                var existingEntity = _dbContext.Set<TEntity>().Find(keyValues)
                    ?? throw new EntityNotFoundException(Error.EntityNotFoundError, nameof(entity));

                _dbContext.Entry(existingEntity).CurrentValues.SetValues(entity);
            }

            _dbContext.SaveChanges();
        }

        public void UpdateWhere(Func<TEntity, bool> predicate, Action<TEntity> updateAction)
        {
            var entitiesToUpdate = _dbContext.Set<TEntity>().Where(predicate).ToList();

            foreach (var entity in entitiesToUpdate)           
                updateAction(entity);
            
            _dbContext.SaveChanges();
        }

        public void Remove(TEntity entityToRemove)
        {
            if (entityToRemove == null)
                throw new NullEntityException(Error.NullEntityError, nameof(entityToRemove));

            _dbContext.Set<TEntity>().Remove(entityToRemove);
            _dbContext.SaveChanges();
        }

        public void RemoveById(int id)
        {
            if (id <= 0)
                throw new NonPositiveIdException(Error.NonPositiveIdError);

            var entityToRemove = _dbContext.Set<TEntity>().Find(id);
            if (entityToRemove == null)
                throw new EntityNotFoundException(Error.EntityNotFoundError, nameof(entityToRemove));

            _dbContext.Set<TEntity>().Remove(entityToRemove);
            _dbContext.SaveChanges();
        }

        public void RemoveRange(IEnumerable<TEntity> entitiesToRemove)
        {
            if (entitiesToRemove == null || entitiesToRemove.Any(x => x == null))
                throw new NullEntityInCollectionException(Error.NullEntityInCollectionError, nameof(entitiesToRemove));

            _dbContext.Set<TEntity>().RemoveRange(entitiesToRemove);
            _dbContext.SaveChanges();
        }

        public void RemoveWhere(Func<TEntity, bool> predicate)
        {
            var entitiesToRemove = _dbContext.Set<TEntity>().Where(predicate).ToList();
            _dbContext.Set<TEntity>().RemoveRange(entitiesToRemove);
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
            catch (Exception ex)
            {
                trans.Rollback();
                throw new TransactionException(Error.TransactionError, ex.Message);
            }
        }

        public IEnumerable<TEntity> ExecuteSqlRaw(string sqlQuery)
        {
            if (string.IsNullOrEmpty(sqlQuery))
                throw new NullSqlQueryException(Error.NullSqlQueryError);

            return _dbContext.Set<TEntity>().FromSqlRaw(sqlQuery).ToList() 
                ?? throw new ExecuteSqlRawException(Error.ExecuteSqlRawError, sqlQuery);
        }

        public int ExecuteSqlRawCommand(string sqlQuery)
        {
            if (string.IsNullOrEmpty(sqlQuery))
                throw new NullSqlQueryException(Error.NullSqlQueryError);

            return _dbContext.Database.ExecuteSqlRaw(sqlQuery);
        }
        #endregion

        #region Async
        public async Task<TEntity?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            if (id <= 0)
                throw new NonPositiveIdException(Error.NonPositiveIdError);

            return await _dbContext.Set<TEntity>().FindAsync([ id ], cancellationToken: cancellationToken);
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync(bool asNoTracking = true, CancellationToken cancellationToken = default)
        {
            return asNoTracking ?
                await _dbContext.Set<TEntity>().AsNoTracking().ToListAsync(cancellationToken) :
                await _dbContext.Set<TEntity>().ToListAsync(cancellationToken);
        }

        public async Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Set<TEntity>().AnyAsync(predicate, cancellationToken);
        }

        public async Task<int> CountAsync(CancellationToken cancellationToken = default)
        {
            return await _dbContext.Set<TEntity>().CountAsync(cancellationToken);
        }

        public async Task<int> CountWhereAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Set<TEntity>().CountAsync(predicate, cancellationToken);
        }

        public async Task AddAsync(TEntity entityToAdd, CancellationToken cancellationToken = default)
        {
            if (entityToAdd == null)
                throw new NullEntityException(Error.NullEntityError, typeof(TEntity).Name);

            await _dbContext.Set<TEntity>().AddAsync(entityToAdd, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task AddRangeAsync(IEnumerable<TEntity> entitiesToAdd, CancellationToken cancellationToken = default)
        {
            if (entitiesToAdd == null || entitiesToAdd.Any(x => x == null))
                throw new NullEntityInCollectionException(Error.NullEntityInCollectionError, typeof(TEntity).Name);

            await _dbContext.Set<TEntity>().AddRangeAsync(entitiesToAdd, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task UpdateAsync(TEntity entityToUpdate, CancellationToken cancellationToken = default)
        {
            if (entityToUpdate == null)
                throw new NullEntityException(Error.NullEntityError, typeof(TEntity).Name);

            var keyValues = _dbContext.Model.FindEntityType(typeof(TEntity))?
                .FindPrimaryKey()?
                .Properties
                .Select(p => p.PropertyInfo?.GetValue(entityToUpdate))
                .ToArray();

            if (keyValues == null || keyValues.Any(k => k == null))
                throw new NullPrimaryKeyException(Error.NullPrimaryKeyError, typeof(TEntity).Name);

            var entity = await _dbContext.Set<TEntity>().FindAsync(keyValues, cancellationToken)
                ?? throw new EntityNotFoundException(Error.EntityNotFoundError, typeof(TEntity).Name);

            _dbContext.Entry(entity).CurrentValues.SetValues(entityToUpdate);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task UpdateByIdAsync(int id, Action<TEntity> updateAction, CancellationToken cancellationToken = default)
        {
            if (id <= 0)
                throw new NonPositiveIdException(Error.NonPositiveIdError);

            var entityToUpdate = await _dbContext.Set<TEntity>().FindAsync([ id ], cancellationToken: cancellationToken) 
                ?? throw new EntityNotFoundException(Error.EntityNotFoundError, typeof(TEntity).Name);

            updateAction(entityToUpdate);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task UpdateRangeAsync(IEnumerable<TEntity> entitiesToUpdate, CancellationToken cancellationToken = default)
        {
            if (entitiesToUpdate == null || !entitiesToUpdate.Any())
                throw new NullEntityInCollectionException(Error.NullEntityInCollectionError, typeof(IEnumerable<TEntity>).Name);

            if (entitiesToUpdate.Any(x => x == null))
                throw new NullEntityInCollectionException(Error.NullEntityInCollectionError, typeof(IEnumerable<TEntity>).Name);

            var primaryKeyProperties = _dbContext.Model.FindEntityType(typeof(TEntity))?
                .FindPrimaryKey()?
                .Properties
                .Select(p => p.PropertyInfo)
                .ToArray();

            if (primaryKeyProperties == null || primaryKeyProperties.Length == 0)
                throw new NullPrimaryKeyException(Error.NullPrimaryKeyError, typeof(IEnumerable<TEntity>).Name);

            foreach (var entity in entitiesToUpdate)
            {
                var keyValues = primaryKeyProperties
                    .Select(p => p.GetValue(entity))
                    .ToArray();

                if (keyValues.Any(k => k == null))
                    throw new ArgumentException("Primary key values cannot be null", typeof(IEnumerable<TEntity>).Name);

                var existingEntity = await _dbContext.Set<TEntity>().FindAsync(keyValues, cancellationToken) 
                    ?? throw new EntityNotFoundException(Error.EntityNotFoundError, nameof(entity));

                _dbContext.Entry(existingEntity).CurrentValues.SetValues(entity);
            }

            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task UpdateWhereAsync(Func<TEntity, bool> predicate, Action<TEntity> updateAction, CancellationToken cancellationToken = default)
        {
            var entitiesToUpdate = _dbContext.Set<TEntity>().Where(predicate).ToList();

            foreach (var entity in entitiesToUpdate)
                updateAction(entity);

            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task RemoveAsync(TEntity entityToRemove, CancellationToken cancellationToken = default)
        {
            if (entityToRemove == null)
                throw new NullEntityException(Error.NullEntityError, nameof(entityToRemove));

            _dbContext.Set<TEntity>().Remove(entityToRemove);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task RemoveByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            if (id <= 0)
                throw new NonPositiveIdException(Error.NonPositiveIdError);

            var entityToRemove = await _dbContext.Set<TEntity>().FindAsync([ id ], cancellationToken: cancellationToken);
            if (entityToRemove == null)
                throw new EntityNotFoundException(Error.EntityNotFoundError, nameof(entityToRemove));

            _dbContext.Set<TEntity>().Remove(entityToRemove);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task RemoveRangeAsync(IEnumerable<TEntity> entitiesToRemove, CancellationToken cancellationToken = default)
        {
            if (entitiesToRemove == null || entitiesToRemove.Any(x => x == null))
                throw new NullEntityInCollectionException(Error.NullEntityInCollectionError, nameof(entitiesToRemove));

            _dbContext.Set<TEntity>().RemoveRange(entitiesToRemove);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task RemoveWhereAsync(Func<TEntity, bool> predicate, CancellationToken cancellationToken = default)
        {
            var entitiesToRemove = _dbContext.Set<TEntity>().Where(predicate).ToList();
            _dbContext.Set<TEntity>().RemoveRange(entitiesToRemove);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task ExecuteTransactionAsync(Func<Task> action, CancellationToken cancellationToken = default)
        {
            using var trans = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                await action();
                await _dbContext.SaveChangesAsync(cancellationToken);
                await trans.CommitAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                await trans.RollbackAsync(cancellationToken);
                throw new TransactionException(Error.TransactionError, ex.Message);
            }
        }

        public async Task<IEnumerable<TEntity>> ExecuteSqlRawAsync(string sqlQuery, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(sqlQuery))
                throw new NullSqlQueryException(Error.NullSqlQueryError);

            return await _dbContext.Set<TEntity>().FromSqlRaw(sqlQuery).ToListAsync(cancellationToken)
                ?? throw new ExecuteSqlRawException(Error.ExecuteSqlRawError, sqlQuery);
        }

        public async Task<int> ExecuteSqlRawCommandAsync(string sqlQuery, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(sqlQuery))
                throw new NullSqlQueryException(Error.NullSqlQueryError);

            return await _dbContext.Database.ExecuteSqlRawAsync(sqlQuery, cancellationToken);
        }

        public async Task<IEnumerable<TEntity>> GetWhereAsync(Expression<Func<TEntity, bool>> predicate, bool asNoTracking = true, CancellationToken cancellationToken = default)
        {
            var query = asNoTracking
                ? _dbContext.Set<TEntity>().AsNoTracking().Where(predicate)
                : _dbContext.Set<TEntity>().Where(predicate);

            return await query.ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<TEntity>> GetOrderedAsync(Expression<Func<TEntity, object>> orderBy, bool isAscending = true, bool asNoTracking = true, CancellationToken cancellationToken = default)
        {
            var query = _dbContext.Set<TEntity>().AsQueryable();

            if (asNoTracking)
                query = query.AsNoTracking();

            query = isAscending
                ? query.OrderBy(orderBy)
                : query.OrderByDescending(orderBy);

            return await query.ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<TEntity>> GetPagedAsync(int pageNumber, int pageSize, Expression<Func<TEntity, object>>? orderBy = null, bool isAscending = true, bool asNoTracking = true, CancellationToken cancellationToken = default)
        {
            if (pageNumber <= 0 || pageSize <= 0)
                throw new GetPageException(Error.GetPageError);

            var query = _dbContext.Set<TEntity>().AsQueryable();

            if (asNoTracking)
                query = query.AsNoTracking();

            if (orderBy != null)
            {
                query = (IQueryable<TEntity>)(isAscending
                    ? query.OrderBy(orderBy)
                    : query.OrderByDescending(orderBy));
            }

            query = query.Skip((pageNumber - 1) * pageSize).Take(pageSize);

            return await query.ToListAsync(cancellationToken);
        }
        #endregion
    }
}
