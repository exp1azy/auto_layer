using AutoLayer.Exceptions;
using AutoLayer.Resources;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace AutoLayer
{
    public class Repository<TEntity>(DbContext dbContext) : IRepository<TEntity> where TEntity : class, new()
    {
        private readonly DbContext _dbContext = dbContext;

        private async Task<TEntity?> ProcessGetById(int id, CancellationToken cancellationToken = default)
        {
            if (id <= 0)
                throw new NonPositiveIdException(Error.NonPositiveIdError);

            return await _dbContext.Set<TEntity>().FindAsync([ id ], cancellationToken: cancellationToken);
        }

        private async Task<IEnumerable<TEntity>> ProcessGetAll(bool asNoTracking = true, CancellationToken cancellationToken = default)
        {
            return asNoTracking ?
                    await _dbContext.Set<TEntity>().AsNoTracking().ToListAsync(cancellationToken) :
                    await _dbContext.Set<TEntity>().ToListAsync(cancellationToken);
        }

        private async Task<TEntity?> ProcessGetFirstWithRelated<TProperty>(Expression<Func<TEntity, bool>> condition, Expression<Func<TEntity, TProperty>> include, bool asNoTracking = true, CancellationToken cancellationToken = default)
        {
            return asNoTracking ?
                await _dbContext.Set<TEntity>().AsNoTracking().Include(include).FirstOrDefaultAsync(condition, cancellationToken) :
                await _dbContext.Set<TEntity>().Include(include).FirstOrDefaultAsync(condition, cancellationToken);
        }

        private async Task<IEnumerable<TEntity>> ProcessGetWithRelated<TProperty>(Expression<Func<TEntity, bool>> condition, Expression<Func<TEntity, TProperty>> include, bool asNoTracking = true, CancellationToken cancellationToken = default)
        {
            return asNoTracking ?
                await _dbContext.Set<TEntity>().AsNoTracking().Include(include).Where(condition).ToListAsync(cancellationToken) :
                await _dbContext.Set<TEntity>().Include(include).Where(condition).ToListAsync(cancellationToken);
        }

        private async Task<bool> ProcessExists(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Set<TEntity>().AnyAsync(predicate, cancellationToken);
        }

        private async Task<bool> ProcessIsEmpty(CancellationToken cancellationToken = default)
        {
            return await _dbContext.Set<TEntity>().AnyAsync(cancellationToken);
        }

        private async Task<IEnumerable<TEntity>> ProcessGetWhere(Expression<Func<TEntity, bool>> predicate, bool asNoTracking = true, CancellationToken cancellationToken = default)
        {
            var query = asNoTracking
                ? _dbContext.Set<TEntity>().AsNoTracking().Where(predicate)
                : _dbContext.Set<TEntity>().Where(predicate);

            return await query.ToListAsync(cancellationToken);
        }

        private async Task<IEnumerable<TEntity>> ProcessGetOrdered(Expression<Func<TEntity, object>> orderBy, bool isAscending = true, bool asNoTracking = true, CancellationToken cancellationToken = default)
        {
            var query = _dbContext.Set<TEntity>().AsQueryable();

            if (asNoTracking)
                query = query.AsNoTracking();

            query = isAscending
                ? query.OrderBy(orderBy)
                : query.OrderByDescending(orderBy);

            return await query.ToListAsync(cancellationToken);
        }

        private async Task<int> ProcessCount(CancellationToken cancellationToken = default)
        {
            return await _dbContext.Set<TEntity>().CountAsync(cancellationToken);
        }

        private async Task<int> ProcessCountWhere(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Set<TEntity>().CountAsync(predicate, cancellationToken);
        }

        private async Task<decimal> ProcessMax(Expression<Func<TEntity, decimal>> selector, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Set<TEntity>().MaxAsync(selector, cancellationToken);
        }

        private async Task<decimal> ProcessMaxWhere(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, decimal>> selector, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Set<TEntity>().Where(predicate).MaxAsync(selector, cancellationToken);
        }

        private async Task<decimal> ProcessMin(Expression<Func<TEntity, decimal>> selector, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Set<TEntity>().MinAsync(selector, cancellationToken);
        }

        private async Task<decimal> ProcessMinWhere(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, decimal>> selector, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Set<TEntity>().Where(predicate).MinAsync(selector, cancellationToken);
        }

        private async Task<decimal> ProcessSum(Expression<Func<TEntity, decimal>> selector, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Set<TEntity>().SumAsync(selector, cancellationToken);
        }

        private async Task<decimal> ProcessSumWhere(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, decimal>> selector, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Set<TEntity>().Where(predicate).SumAsync(selector, cancellationToken);
        }

        private async Task<decimal> ProcessAverage(Expression<Func<TEntity, decimal>> selector, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Set<TEntity>().AverageAsync(selector, cancellationToken);
        }

        private async Task<decimal> ProcessAverageWhere(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, decimal>> selector, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Set<TEntity>().Where(predicate).AverageAsync(selector, cancellationToken);
        }

        private async Task<IEnumerable<TEntity>> ProcessGetPaged(int pageNumber, int pageSize, Expression<Func<TEntity, object>>? orderBy = null, bool isAscending = true, bool asNoTracking = true, CancellationToken cancellationToken = default)
        {
            if (pageNumber <= 0 || pageSize <= 0)
                throw new GetPageException(Error.GetPageError);

            var query = _dbContext.Set<TEntity>().AsQueryable();

            if (asNoTracking)
                query = query.AsNoTracking();

            if (orderBy != null)
            {
                query = isAscending
                    ? query.OrderBy(orderBy)
                    : query.OrderByDescending(orderBy);
            }

            query = query.Skip((pageNumber - 1) * pageSize).Take(pageSize);

            return await query.ToListAsync(cancellationToken);
        }

        private async Task ProcessAdd(TEntity entityToAdd, CancellationToken cancellationToken = default)
        {
            if (entityToAdd == null)
                throw new NullEntityException(Error.NullEntityError, typeof(TEntity).Name);

            await _dbContext.Set<TEntity>().AddAsync(entityToAdd, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        private async Task ProcessAddRange(IEnumerable<TEntity> entitiesToAdd, CancellationToken cancellationToken = default)
        {
            if (entitiesToAdd == null || entitiesToAdd.Any(x => x == null))
                throw new NullEntityInCollectionException(Error.NullEntityInCollectionError, typeof(TEntity).Name);

            await _dbContext.Set<TEntity>().AddRangeAsync(entitiesToAdd, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        private async Task ProcessUpdate(TEntity entityToUpdate, CancellationToken cancellationToken = default)
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

        private async Task ProcessUpdateById(int id, Action<TEntity> updateAction, CancellationToken cancellationToken = default)
        {
            if (id <= 0)
                throw new NonPositiveIdException(Error.NonPositiveIdError);

            var entityToUpdate = await _dbContext.Set<TEntity>().FindAsync([id], cancellationToken: cancellationToken)
                ?? throw new EntityNotFoundException(Error.EntityNotFoundError, typeof(TEntity).Name);

            updateAction(entityToUpdate);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        private async Task ProcessUpdateRange(IEnumerable<TEntity> entitiesToUpdate, CancellationToken cancellationToken = default)
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
                    throw new NullPrimaryKeyException(Error.NullPrimaryKeyError, typeof(TEntity).Name);

                var existingEntity = await _dbContext.Set<TEntity>().FindAsync(keyValues, cancellationToken)
                    ?? throw new EntityNotFoundException(Error.EntityNotFoundError, nameof(entity));

                _dbContext.Entry(existingEntity).CurrentValues.SetValues(entity);
            }

            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        private async Task ProcessUpdateWhere(Func<TEntity, bool> predicate, Action<TEntity> updateAction, CancellationToken cancellationToken = default)
        {
            var entitiesToUpdate = _dbContext.Set<TEntity>().Where(predicate).ToList();

            foreach (var entity in entitiesToUpdate)
                updateAction(entity);

            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        private async Task ProcessRemove(TEntity entityToRemove, CancellationToken cancellationToken = default)
        {
            if (entityToRemove == null)
                throw new NullEntityException(Error.NullEntityError, nameof(entityToRemove));

            _dbContext.Set<TEntity>().Remove(entityToRemove);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        private async Task ProcessRemoveById(int id, CancellationToken cancellationToken = default)
        {
            if (id <= 0)
                throw new NonPositiveIdException(Error.NonPositiveIdError);

            var entityToRemove = await _dbContext.Set<TEntity>().FindAsync([id], cancellationToken: cancellationToken);
            if (entityToRemove == null)
                throw new EntityNotFoundException(Error.EntityNotFoundError, nameof(entityToRemove));

            _dbContext.Set<TEntity>().Remove(entityToRemove);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        private async Task ProcessRemoveRange(IEnumerable<TEntity> entitiesToRemove, CancellationToken cancellationToken = default)
        {
            if (entitiesToRemove == null || entitiesToRemove.Any(x => x == null))
                throw new NullEntityInCollectionException(Error.NullEntityInCollectionError, nameof(entitiesToRemove));

            _dbContext.Set<TEntity>().RemoveRange(entitiesToRemove);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        private async Task ProcessRemoveWhere(Func<TEntity, bool> predicate, CancellationToken cancellationToken = default)
        {
            var entitiesToRemove = _dbContext.Set<TEntity>().Where(predicate).ToList();
            _dbContext.Set<TEntity>().RemoveRange(entitiesToRemove);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        private async Task ProcessTransactExecution(Func<Task> action, CancellationToken cancellationToken = default)
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

        private async Task<IEnumerable<TEntity>> ProcessSqlRawExecution(string sqlQuery, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(sqlQuery))
                throw new NullSqlQueryException(Error.NullSqlQueryError);

            return await _dbContext.Set<TEntity>().FromSqlRaw(sqlQuery).ToListAsync(cancellationToken)
                ?? throw new ExecuteSqlRawException(Error.ExecuteSqlRawError, sqlQuery);
        }

        private async Task<int> ProcessSqlRawCommandExecution(string sqlQuery, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(sqlQuery))
                throw new NullSqlQueryException(Error.NullSqlQueryError);

            return await _dbContext.Database.ExecuteSqlRawAsync(sqlQuery, cancellationToken);
        }

        #region Sync

        public TEntity? GetById(int id) => 
            ProcessGetById(id).GetAwaiter().GetResult();

        public IEnumerable<TEntity> GetAll(bool asNoTracking = true) =>
            ProcessGetAll(asNoTracking).GetAwaiter().GetResult();

        public IEnumerable<TEntity> GetWhere(Expression<Func<TEntity, bool>> predicate, bool asNoTracking = true) =>
            ProcessGetWhere(predicate, asNoTracking).GetAwaiter().GetResult();

        public TEntity? GetFirstWithRelated<TProperty>(Expression<Func<TEntity, bool>> condition, Expression<Func<TEntity, TProperty>> include, bool asNoTracking = true) =>
            ProcessGetFirstWithRelated(condition, include, asNoTracking).GetAwaiter().GetResult();

        public IEnumerable<TEntity> GetWithRelated<TProperty>(Expression<Func<TEntity, bool>> condition, Expression<Func<TEntity, TProperty>> include, bool asNoTracking = true) =>
            ProcessGetWithRelated(condition, include, asNoTracking).GetAwaiter().GetResult();

        public bool Exists(Expression<Func<TEntity, bool>> predicate) =>
            ProcessExists(predicate).GetAwaiter().GetResult();

        public bool IsEmpty() =>
            ProcessIsEmpty().GetAwaiter().GetResult();

        public int Count() =>
            ProcessCount().GetAwaiter().GetResult();

        public int CountWhere(Expression<Func<TEntity, bool>> predicate) =>
            ProcessCountWhere(predicate).GetAwaiter().GetResult();

        public decimal Max(Expression<Func<TEntity, decimal>> selector) =>
            ProcessMax(selector).GetAwaiter().GetResult();

        public decimal MaxWhere(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, decimal>> selector) =>
            ProcessMaxWhere(predicate, selector).GetAwaiter().GetResult();

        public decimal Min(Expression<Func<TEntity, decimal>> selector) =>
            ProcessMin(selector).GetAwaiter().GetResult();

        public decimal MinWhere(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, decimal>> selector) =>
            ProcessMinWhere(predicate, selector).GetAwaiter().GetResult();

        public decimal Sum(Expression<Func<TEntity, decimal>> selector) =>
            ProcessSum(selector).GetAwaiter().GetResult();

        public decimal SumWhere(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, decimal>> selector) =>
            ProcessSumWhere(predicate, selector).GetAwaiter().GetResult();

        public decimal Average(Expression<Func<TEntity, decimal>> selector) =>
            ProcessAverage(selector).GetAwaiter().GetResult();

        public decimal AverageWhere(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, decimal>> selector) =>
            ProcessAverageWhere(predicate, selector).GetAwaiter().GetResult();

        public IQueryable<TEntity> GetQuery() =>
            _dbContext.Set<TEntity>().AsQueryable();

        public IEnumerable<TEntity> GetOrdered(Expression<Func<TEntity, object>> orderBy, bool isAscending = true, bool asNoTracking = true) =>
            ProcessGetOrdered(orderBy, isAscending, asNoTracking).GetAwaiter().GetResult();

        public IEnumerable<TEntity> GetPaged(int pageNumber, int pageSize, Expression<Func<TEntity, object>>? orderBy = null, bool isAscending = true, bool asNoTracking = true) =>
            ProcessGetPaged(pageNumber, pageSize, orderBy, isAscending, asNoTracking).GetAwaiter().GetResult();      

        public void Add(TEntity entityToAdd) =>
            ProcessAdd(entityToAdd).GetAwaiter().GetResult();

        public void AddRange(IEnumerable<TEntity> entitiesToAdd) =>
            ProcessAddRange(entitiesToAdd).GetAwaiter().GetResult();

        public void Update(TEntity entityToUpdate) =>
            ProcessUpdate(entityToUpdate).GetAwaiter().GetResult();

        public void UpdateById(int id, Action<TEntity> updateAction) =>
            ProcessUpdateById(id, updateAction).GetAwaiter().GetResult();

        public void UpdateRange(IEnumerable<TEntity> entitiesToUpdate) =>
            ProcessUpdateRange(entitiesToUpdate).GetAwaiter().GetResult();

        public void UpdateWhere(Func<TEntity, bool> predicate, Action<TEntity> updateAction) =>
            ProcessUpdateWhere(predicate, updateAction).GetAwaiter().GetResult();

        public void Remove(TEntity entityToRemove) =>
            ProcessRemove(entityToRemove).GetAwaiter().GetResult();

        public void RemoveById(int id) =>
            ProcessRemoveById(id).GetAwaiter().GetResult();

        public void RemoveRange(IEnumerable<TEntity> entitiesToRemove) =>
            ProcessRemoveRange(entitiesToRemove).GetAwaiter().GetResult();

        public void RemoveWhere(Func<TEntity, bool> predicate) =>
            ProcessRemoveWhere(predicate).GetAwaiter().GetResult();

        public void ExecuteTransaction(Func<Task> action) =>
            ProcessTransactExecution(action).GetAwaiter().GetResult();

        public IEnumerable<TEntity> ExecuteSqlRaw(string sqlQuery) =>
            ProcessSqlRawExecution(sqlQuery).GetAwaiter().GetResult();

        public int ExecuteSqlRawCommand(string sqlQuery) =>
            ProcessSqlRawCommandExecution(sqlQuery).GetAwaiter().GetResult();

        #endregion

        #region Async

        public async Task<TEntity?> GetByIdAsync(int id, CancellationToken cancellationToken = default) =>
            await ProcessGetById(id, cancellationToken);

        public async Task<IEnumerable<TEntity>> GetAllAsync(bool asNoTracking = true, CancellationToken cancellationToken = default) =>
            await ProcessGetAll(asNoTracking, cancellationToken);

        public async Task<IEnumerable<TEntity>> GetWhereAsync(Expression<Func<TEntity, bool>> predicate, bool asNoTracking = true, CancellationToken cancellationToken = default) =>
            await ProcessGetWhere(predicate, asNoTracking, cancellationToken);

        public async Task<TEntity?> GetFirstWithRelatedAsync<TProperty>(Expression<Func<TEntity, bool>> condition, Expression<Func<TEntity, TProperty>> include, bool asNoTracking = true, CancellationToken cancellationToken = default) =>
            await ProcessGetFirstWithRelated(condition, include, asNoTracking, cancellationToken);

        public async Task<IEnumerable<TEntity>> GetWithRelatedAsync<TProperty>(Expression<Func<TEntity, bool>> condition, Expression<Func<TEntity, TProperty>> include, bool asNoTracking = true, CancellationToken cancellationToken = default) =>
            await ProcessGetWithRelated(condition, include, asNoTracking, cancellationToken);

        public async Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default) =>
            await ProcessExists(predicate, cancellationToken);

        public async Task<bool> IsEmptyAsync(CancellationToken cancellationToken = default) =>
            await ProcessIsEmpty(cancellationToken);

        public async Task<int> CountAsync(CancellationToken cancellationToken = default) =>
            await ProcessCount(cancellationToken);

        public async Task<int> CountWhereAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default) =>
            await ProcessCountWhere(predicate, cancellationToken);

        public async Task<decimal> MaxAsync(Expression<Func<TEntity, decimal>> selector, CancellationToken cancellationToken = default) =>
            await ProcessMax(selector, cancellationToken);

        public async Task<decimal> MaxWhereAsync(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, decimal>> selector, CancellationToken cancellationToken = default) =>
            await ProcessMaxWhere(predicate, selector, cancellationToken);

        public async Task<decimal> MinAsync(Expression<Func<TEntity, decimal>> selector, CancellationToken cancellationToken = default) =>
            await ProcessMin(selector, cancellationToken);

        public async Task<decimal> MinWhereAsync(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, decimal>> selector, CancellationToken cancellationToken = default) =>
            await ProcessMinWhere(predicate, selector, cancellationToken);

        public async Task<decimal> SumAsync(Expression<Func<TEntity, decimal>> selector, CancellationToken cancellationToken = default) =>
            await ProcessSum(selector, cancellationToken);

        public async Task<decimal> SumWhereAsync(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, decimal>> selector, CancellationToken cancellationToken = default) =>
            await ProcessSumWhere(predicate, selector, cancellationToken);

        public async Task<decimal> AverageAsync(Expression<Func<TEntity, decimal>> selector, CancellationToken cancellationToken = default) =>
            await ProcessAverage(selector, cancellationToken);

        public async Task<decimal> AverageWhereAsync(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, decimal>> selector, CancellationToken cancellationToken = default) =>
            await ProcessAverageWhere(predicate, selector, cancellationToken);

        public async Task<IEnumerable<TEntity>> GetOrderedAsync(Expression<Func<TEntity, object>> orderBy, bool isAscending = true, bool asNoTracking = true, CancellationToken cancellationToken = default) =>
            await ProcessGetOrdered(orderBy, isAscending, asNoTracking, cancellationToken);

        public async Task<IEnumerable<TEntity>> GetPagedAsync(int pageNumber, int pageSize, Expression<Func<TEntity, object>>? orderBy = null, bool isAscending = true, bool asNoTracking = true, CancellationToken cancellationToken = default) =>
            await ProcessGetPaged(pageNumber, pageSize, orderBy, isAscending, asNoTracking, cancellationToken);

        public async Task AddAsync(TEntity entityToAdd, CancellationToken cancellationToken = default) =>
            await ProcessAdd(entityToAdd, cancellationToken);

        public async Task AddRangeAsync(IEnumerable<TEntity> entitiesToAdd, CancellationToken cancellationToken = default) =>
            await ProcessAddRange(entitiesToAdd, cancellationToken);

        public async Task UpdateAsync(TEntity entityToUpdate, CancellationToken cancellationToken = default) =>
            await ProcessUpdate(entityToUpdate, cancellationToken);

        public async Task UpdateByIdAsync(int id, Action<TEntity> updateAction, CancellationToken cancellationToken = default) =>
            await ProcessUpdateById(id, updateAction, cancellationToken);

        public async Task UpdateRangeAsync(IEnumerable<TEntity> entitiesToUpdate, CancellationToken cancellationToken = default) =>
            await ProcessUpdateRange(entitiesToUpdate, cancellationToken);

        public async Task UpdateWhereAsync(Func<TEntity, bool> predicate, Action<TEntity> updateAction, CancellationToken cancellationToken = default) =>
            await ProcessUpdateWhere(predicate, updateAction, cancellationToken);

        public async Task RemoveAsync(TEntity entityToRemove, CancellationToken cancellationToken = default) =>
            await ProcessRemove(entityToRemove, cancellationToken);

        public async Task RemoveByIdAsync(int id, CancellationToken cancellationToken = default) =>
            await ProcessRemoveById(id, cancellationToken);

        public async Task RemoveRangeAsync(IEnumerable<TEntity> entitiesToRemove, CancellationToken cancellationToken = default) =>
            await ProcessRemoveRange(entitiesToRemove, cancellationToken);

        public async Task RemoveWhereAsync(Func<TEntity, bool> predicate, CancellationToken cancellationToken = default) =>
            await ProcessRemoveWhere(predicate, cancellationToken);

        public async Task ExecuteTransactionAsync(Func<Task> action, CancellationToken cancellationToken = default) =>
            await ProcessTransactExecution(action, cancellationToken);

        public async Task<IEnumerable<TEntity>> ExecuteSqlRawAsync(string sqlQuery, CancellationToken cancellationToken = default) =>
            await ProcessSqlRawExecution(sqlQuery, cancellationToken);

        public async Task<int> ExecuteSqlRawCommandAsync(string sqlQuery, CancellationToken cancellationToken = default) =>
            await ProcessSqlRawCommandExecution(sqlQuery, cancellationToken);

        #endregion
    }
}
