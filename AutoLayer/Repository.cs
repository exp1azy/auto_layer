using AutoLayer.Exceptions;
using AutoLayer.Resources;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace AutoLayer
{
    /// <summary>
    /// A generic repository class for performing CRUD operations and interacting with the database.
    /// Provides a base implementation for working with entities in a given <see cref="DbContext"/>.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity to operate on. Must be a reference type and have a parameterless constructor.</typeparam>
    public class Repository<TEntity>(DbContext dbContext) where TEntity : class, new()
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

        private async Task<TEntity?> ProcessGetFirst(Expression<Func<TEntity, bool>> predicate, bool asNoTracking = true, CancellationToken cancellationToken = default)
        {
            return asNoTracking
                ? await _dbContext.Set<TEntity>().AsNoTracking().FirstOrDefaultAsync(predicate, cancellationToken)
                : await _dbContext.Set<TEntity>().FirstOrDefaultAsync(predicate, cancellationToken);
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

        /// <summary>
        /// Retrieves an entity by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the entity.</param>
        /// <returns>The entity with the specified identifier, or <c>null</c> if not found.</returns>
        public TEntity? GetById(int id) =>
            ProcessGetById(id).GetAwaiter().GetResult();

        /// <summary>
        /// Retrieves all entities of the specified type from the database.
        /// </summary>
        /// <param name="asNoTracking">
        /// A flag indicating whether to retrieve entities without tracking them in the context. 
        /// Set to <c>true</c> to disable tracking, improving performance for read-only operations.
        /// </param>
        /// <returns>A collection of all entities.</returns>
        public IEnumerable<TEntity> GetAll(bool asNoTracking = true) =>
            ProcessGetAll(asNoTracking).GetAwaiter().GetResult();

        /// <summary>
        /// Retrieves the first entity that matches the specified condition.
        /// </summary>
        /// <param name="predicate">An expression that defines the condition to filter the entities.</param>
        /// <param name="asNoTracking">
        /// A flag indicating whether to retrieve the entity without tracking it in the context. 
        /// Set to <c>true</c> to disable tracking, improving performance for read-only operations.
        /// </param>
        /// <returns>The first entity matching the condition, or <c>null</c> if no entity is found.</returns>
        public TEntity? GetFirst(Expression<Func<TEntity, bool>> predicate, bool asNoTracking = true) =>
            ProcessGetFirst(predicate, asNoTracking).GetAwaiter().GetResult();

        /// <summary>
        /// Retrieves all entities that match the specified condition.
        /// </summary>
        /// <param name="predicate">An expression that defines the condition to filter the entities.</param>
        /// <param name="asNoTracking">
        /// A flag indicating whether to retrieve entities without tracking them in the context. 
        /// Set to <c>true</c> to disable tracking, improving performance for read-only operations.
        /// </param>
        /// <returns>A collection of entities matching the condition.</returns>
        public IEnumerable<TEntity> GetWhere(Expression<Func<TEntity, bool>> predicate, bool asNoTracking = true) =>
            ProcessGetWhere(predicate, asNoTracking).GetAwaiter().GetResult();

        /// <summary>
        /// Retrieves the first entity that matches the specified condition and includes related data.
        /// </summary>
        /// <typeparam name="TProperty">The type of the related property to include.</typeparam>
        /// <param name="condition">An expression specifying the condition to filter the entities.</param>
        /// <param name="include">An expression specifying the related property to include in the query.</param>
        /// <param name="asNoTracking">
        /// A flag indicating whether to retrieve the entity without tracking it in the context.
        /// Set to <c>true</c> for improved performance in read-only operations.
        /// </param>
        /// <returns>The first entity matching the condition with the specified related data included, or <c>null</c> if no entity is found.</returns>
        public TEntity? GetFirstWithRelated<TProperty>(
            Expression<Func<TEntity, bool>> condition,
            Expression<Func<TEntity, TProperty>> include,
            bool asNoTracking = true) =>
            ProcessGetFirstWithRelated(condition, include, asNoTracking).GetAwaiter().GetResult();

        /// <summary>
        /// Retrieves all entities that match the specified condition and includes related data.
        /// </summary>
        /// <typeparam name="TProperty">The type of the related property to include.</typeparam>
        /// <param name="condition">An expression specifying the condition to filter the entities.</param>
        /// <param name="include">An expression specifying the related property to include in the query.</param>
        /// <param name="asNoTracking">
        /// A flag indicating whether to retrieve entities without tracking them in the context.
        /// Set to <c>true</c> for improved performance in read-only operations.
        /// </param>
        /// <returns>A collection of entities matching the condition with the specified related data included.</returns>
        public IEnumerable<TEntity> GetWithRelated<TProperty>(
            Expression<Func<TEntity, bool>> condition,
            Expression<Func<TEntity, TProperty>> include,
            bool asNoTracking = true) =>
            ProcessGetWithRelated(condition, include, asNoTracking).GetAwaiter().GetResult();

        /// <summary>
        /// Checks if any entity satisfies the specified condition.
        /// </summary>
        /// <param name="predicate">An expression specifying the condition to check against entities.</param>
        /// <returns><c>true</c> if any entity matches the condition; otherwise, <c>false</c>.</returns>
        public bool Exists(Expression<Func<TEntity, bool>> predicate) =>
            ProcessExists(predicate).GetAwaiter().GetResult();

        /// <summary>
        /// Checks if the repository is empty, i.e., contains no entities.
        /// </summary>
        /// <returns><c>true</c> if the repository contains no entities; otherwise, <c>false</c>.</returns>
        public bool IsEmpty() =>
            ProcessIsEmpty().GetAwaiter().GetResult();

        /// <summary>
        /// Retrieves the total number of entities in the repository.
        /// </summary>
        /// <returns>The total count of entities.</returns>
        public int Count() =>
            ProcessCount().GetAwaiter().GetResult();

        /// <summary>
        /// Counts the number of entities that satisfy the specified condition.
        /// </summary>
        /// <param name="predicate">An expression specifying the condition to filter entities.</param>
        /// <returns>The count of entities that match the condition.</returns>
        public int CountWhere(Expression<Func<TEntity, bool>> predicate) =>
            ProcessCountWhere(predicate).GetAwaiter().GetResult();

        /// <summary>
        /// Retrieves the maximum value of a specified property across all entities.
        /// </summary>
        /// <param name="selector">An expression specifying the property to evaluate for the maximum value.</param>
        /// <returns>The maximum value of the specified property.</returns>
        public decimal Max(Expression<Func<TEntity, decimal>> selector) =>
            ProcessMax(selector).GetAwaiter().GetResult();

        /// <summary>
        /// Retrieves the maximum value of a specified property for entities that satisfy a given condition.
        /// </summary>
        /// <param name="predicate">An expression specifying the condition to filter entities.</param>
        /// <param name="selector">An expression specifying the property to evaluate for the maximum value.</param>
        /// <returns>The maximum value of the specified property among entities that match the condition.</returns>
        public decimal MaxWhere(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, decimal>> selector) =>
            ProcessMaxWhere(predicate, selector).GetAwaiter().GetResult();

        /// <summary>
        /// Retrieves the minimum value of a specified property across all entities.
        /// </summary>
        /// <param name="selector">An expression specifying the property to evaluate for the minimum value.</param>
        /// <returns>The minimum value of the specified property.</returns>
        public decimal Min(Expression<Func<TEntity, decimal>> selector) =>
            ProcessMin(selector).GetAwaiter().GetResult();

        /// <summary>
        /// Retrieves the minimum value of a specified property for entities that satisfy a given condition.
        /// </summary>
        /// <param name="predicate">An expression specifying the condition to filter entities.</param>
        /// <param name="selector">An expression specifying the property to evaluate for the minimum value.</param>
        /// <returns>The minimum value of the specified property among entities that match the condition.</returns>
        public decimal MinWhere(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, decimal>> selector) =>
            ProcessMinWhere(predicate, selector).GetAwaiter().GetResult();

        /// <summary>
        /// Calculates the sum of a specified property across all entities.
        /// </summary>
        /// <param name="selector">An expression specifying the property to sum.</param>
        /// <returns>The total sum of the specified property.</returns>
        public decimal Sum(Expression<Func<TEntity, decimal>> selector) =>
            ProcessSum(selector).GetAwaiter().GetResult();

        /// <summary>
        /// Calculates the sum of a specified property for entities that satisfy a given condition.
        /// </summary>
        /// <param name="predicate">An expression specifying the condition to filter entities.</param>
        /// <param name="selector">An expression specifying the property to sum.</param>
        /// <returns>The total sum of the specified property among entities that match the condition.</returns>
        public decimal SumWhere(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, decimal>> selector) =>
            ProcessSumWhere(predicate, selector).GetAwaiter().GetResult();

        /// <summary>
        /// Calculates the average value of a specified property across all entities.
        /// </summary>
        /// <param name="selector">An expression specifying the property to calculate the average for.</param>
        /// <returns>The average value of the specified property.</returns>
        public decimal Average(Expression<Func<TEntity, decimal>> selector) =>
            ProcessAverage(selector).GetAwaiter().GetResult();

        /// <summary>
        /// Calculates the average value of a specified property for entities that satisfy a given condition.
        /// </summary>
        /// <param name="predicate">An expression specifying the condition to filter entities.</param>
        /// <param name="selector">An expression specifying the property to calculate the average for.</param>
        /// <returns>The average value of the specified property among entities that match the condition.</returns>
        public decimal AverageWhere(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, decimal>> selector) =>
            ProcessAverageWhere(predicate, selector).GetAwaiter().GetResult();

        /// <summary>
        /// Retrieves an <see cref="IQueryable{TEntity}"/> representing the current set of entities, allowing for advanced query composition.
        /// </summary>
        /// <returns>An <see cref="IQueryable{TEntity}"/> for building and executing LINQ queries.</returns>
        public IQueryable<TEntity> GetQuery() =>
            _dbContext.Set<TEntity>().AsQueryable();

        /// <summary>
        /// Retrieves entities ordered by a specified property, with optional ordering direction and tracking settings.
        /// </summary>
        /// <param name="orderBy">An expression specifying the property to order entities by.</param>
        /// <param name="isAscending">Specifies the ordering direction. Default is ascending.</param>
        /// <param name="asNoTracking">Determines whether the entities should be retrieved without tracking. Default is <c>true</c>.</param>
        /// <returns>An <see cref="IEnumerable{TEntity}"/> of ordered entities.</returns>
        public IEnumerable<TEntity> GetOrdered(Expression<Func<TEntity, object>> orderBy, bool isAscending = true, bool asNoTracking = true) =>
            ProcessGetOrdered(orderBy, isAscending, asNoTracking).GetAwaiter().GetResult();

        /// <summary>
        /// Retrieves a paginated collection of entities with optional sorting and tracking settings.
        /// </summary>
        /// <param name="pageNumber">The page number to retrieve. Must be greater than or equal to 1.</param>
        /// <param name="pageSize">The number of entities per page. Must be greater than 0.</param>
        /// <param name="orderBy">An optional expression specifying the property to sort by. Defaults to <c>null</c>.</param>
        /// <param name="isAscending">Specifies the sorting direction. Default is ascending.</param>
        /// <param name="asNoTracking">Determines whether the entities should be retrieved without tracking. Default is <c>true</c>.</param>
        /// <returns>An <see cref="IEnumerable{TEntity}"/> representing the requested page of entities.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="pageNumber"/> or <paramref name="pageSize"/> is less than the required minimum values.</exception>
        public IEnumerable<TEntity> GetPaged(int pageNumber, int pageSize, Expression<Func<TEntity, object>>? orderBy = null, bool isAscending = true, bool asNoTracking = true) =>
            ProcessGetPaged(pageNumber, pageSize, orderBy, isAscending, asNoTracking).GetAwaiter().GetResult();

        /// <summary>
        /// Adds a new entity to the database context.
        /// </summary>
        /// <param name="entityToAdd">The entity to be added.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="entityToAdd"/> is <c>null</c>.</exception>
        /// <remarks>
        /// The changes will be persisted to the database upon calling <c>SaveChanges</c> on the <see cref="DbContext"/>.
        /// </remarks>
        public void Add(TEntity entityToAdd) =>
            ProcessAdd(entityToAdd).GetAwaiter().GetResult();

        /// <summary>
        /// Adds a collection of entities to the database context.
        /// </summary>
        /// <param name="entitiesToAdd">The collection of entities to be added.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="entitiesToAdd"/> is <c>null</c>.</exception>
        /// <remarks>
        /// The changes will be persisted to the database upon calling <c>SaveChanges</c> on the <see cref="DbContext"/>.
        /// </remarks>
        public void AddRange(IEnumerable<TEntity> entitiesToAdd) =>
            ProcessAddRange(entitiesToAdd).GetAwaiter().GetResult();

        /// <summary>
        /// Updates an existing entity in the database context.
        /// </summary>
        /// <param name="entityToUpdate">The entity with updated values.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="entityToUpdate"/> is <c>null</c>.</exception>
        /// <remarks>
        /// The changes will be persisted to the database upon calling <c>SaveChanges</c> on the <see cref="DbContext"/>.
        /// Ensure the entity is tracked by the context before calling this method.
        /// </remarks>
        public void Update(TEntity entityToUpdate) =>
            ProcessUpdate(entityToUpdate).GetAwaiter().GetResult();

        /// <summary>
        /// Updates an entity identified by its ID with the specified update action.
        /// </summary>
        /// <param name="id">The ID of the entity to update.</param>
        /// <param name="updateAction">An action that performs the update on the entity.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="updateAction"/> is <c>null</c>.</exception>
        /// <exception cref="KeyNotFoundException">Thrown if no entity with the specified <paramref name="id"/> is found.</exception>
        /// <remarks>
        /// The entity is fetched by its ID, updated with the specified action, and then persisted to the database upon calling <c>SaveChanges</c> on the <see cref="DbContext"/>.
        /// </remarks>
        public void UpdateById(int id, Action<TEntity> updateAction) =>
            ProcessUpdateById(id, updateAction).GetAwaiter().GetResult();

        /// <summary>
        /// Updates a collection of entities in the database.
        /// </summary>
        /// <param name="entitiesToUpdate">The collection of entities to update.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="entitiesToUpdate"/> is <c>null</c>.</exception>
        /// <remarks>
        /// The changes will be persisted to the database upon calling <c>SaveChanges</c> on the <see cref="DbContext"/>.
        /// </remarks>
        public void UpdateRange(IEnumerable<TEntity> entitiesToUpdate) =>
            ProcessUpdateRange(entitiesToUpdate).GetAwaiter().GetResult();

        /// <summary>
        /// Updates entities that match the specified predicate with the provided update action.
        /// </summary>
        /// <param name="predicate">A function to filter the entities that need to be updated.</param>
        /// <param name="updateAction">An action that applies updates to the selected entities.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="updateAction"/> or <paramref name="predicate"/> is <c>null</c>.</exception>
        /// <remarks>
        /// The entities matching the predicate are fetched, updated with the specified action, and then persisted to the database.
        /// </remarks>
        public void UpdateWhere(Func<TEntity, bool> predicate, Action<TEntity> updateAction) =>
            ProcessUpdateWhere(predicate, updateAction).GetAwaiter().GetResult();

        /// <summary>
        /// Removes an entity from the database context.
        /// </summary>
        /// <param name="entityToRemove">The entity to remove from the database.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="entityToRemove"/> is <c>null</c>.</exception>
        /// <remarks>
        /// The entity will be deleted from the database when <c>SaveChanges</c> is called on the <see cref="DbContext"/>.
        /// </remarks>
        public void Remove(TEntity entityToRemove) =>
            ProcessRemove(entityToRemove).GetAwaiter().GetResult();

        /// <summary>
        /// Removes an entity identified by its ID from the database.
        /// </summary>
        /// <param name="id">The ID of the entity to remove.</param>
        /// <exception cref="ArgumentException">Thrown if the entity with the specified ID is not found.</exception>
        /// <remarks>
        /// The entity with the specified ID is removed from the database upon calling <c>SaveChanges</c> on the <see cref="DbContext"/>.
        /// </remarks>
        public void RemoveById(int id) =>
            ProcessRemoveById(id).GetAwaiter().GetResult();

        /// <summary>
        /// Removes a collection of entities from the database.
        /// </summary>
        /// <param name="entitiesToRemove">The collection of entities to remove from the database.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="entitiesToRemove"/> is <c>null</c>.</exception>
        /// <remarks>
        /// The provided entities are removed from the database upon calling <c>SaveChanges</c> on the <see cref="DbContext"/>.
        /// </remarks>
        public void RemoveRange(IEnumerable<TEntity> entitiesToRemove) =>
            ProcessRemoveRange(entitiesToRemove).GetAwaiter().GetResult();

        /// <summary>
        /// Removes entities from the database that match the specified predicate.
        /// </summary>
        /// <param name="predicate">A function to filter the entities to be removed.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="predicate"/> is <c>null</c>.</exception>
        /// <remarks>
        /// Entities that match the provided predicate will be removed from the database when <c>SaveChanges</c> is called.
        /// </remarks>
        public void RemoveWhere(Func<TEntity, bool> predicate) =>
            ProcessRemoveWhere(predicate).GetAwaiter().GetResult();

        /// <summary>
        /// Executes a transaction with the provided action as part of the transaction scope.
        /// </summary>
        /// <param name="action">A function that represents the action to be executed within the transaction.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="action"/> is <c>null</c>.</exception>
        /// <remarks>
        /// The provided action is executed within a database transaction. If any exception occurs during execution,
        /// the transaction will be rolled back. The action is committed if it completes successfully.
        /// </remarks>
        public void ExecuteTransaction(Func<Task> action) =>
            ProcessTransactExecution(action).GetAwaiter().GetResult();

        /// <summary>
        /// Executes a raw SQL query and returns a collection of entities based on the result.
        /// </summary>
        /// <param name="sqlQuery">The raw SQL query to execute.</param>
        /// <returns>An <see cref="IEnumerable{TEntity}"/> representing the entities returned by the query.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="sqlQuery"/> is <c>null</c> or empty.</exception>
        /// <remarks>
        /// This method allows execution of raw SQL queries that return a collection of entities. The query results are mapped to the corresponding entity type.
        /// </remarks>
        public IEnumerable<TEntity> ExecuteSqlRaw(string sqlQuery) =>
            ProcessSqlRawExecution(sqlQuery).GetAwaiter().GetResult();

        /// <summary>
        /// Executes a raw SQL query that does not return any data, such as an UPDATE or DELETE operation.
        /// </summary>
        /// <param name="sqlQuery">The raw SQL query to execute.</param>
        /// <returns>The number of rows affected by the query.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="sqlQuery"/> is <c>null</c> or empty.</exception>
        /// <remarks>
        /// This method is useful for executing SQL commands that modify the database, such as INSERT, UPDATE, DELETE, etc.
        /// The result indicates the number of rows affected by the execution of the query.
        /// </remarks>
        public int ExecuteSqlRawCommand(string sqlQuery) =>
            ProcessSqlRawCommandExecution(sqlQuery).GetAwaiter().GetResult();


        #endregion

        #region Async

        /// <summary>
        /// Asynchronously retrieves an entity by its identifier.
        /// </summary>
        /// <param name="id">The identifier of the entity to retrieve.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A task representing the asynchronous operation, with a result of the entity, or <c>null</c> if not found.</returns>
        /// <remarks>
        /// This method allows the retrieval of an entity based on its identifier in an asynchronous manner.
        /// If no entity is found with the given identifier, <c>null</c> will be returned.
        /// </remarks>
        public async Task<TEntity?> GetByIdAsync(int id, CancellationToken cancellationToken = default) =>
            await ProcessGetById(id, cancellationToken);

        /// <summary>
        /// Asynchronously retrieves all entities, optionally with or without tracking.
        /// </summary>
        /// <param name="asNoTracking">Specifies whether to disable entity tracking for performance improvements. Defaults to <c>true</c>.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A task representing the asynchronous operation, with a result of a collection of all entities.</returns>
        /// <remarks>
        /// This method retrieves all entities asynchronously, with an option to disable entity tracking for read-only scenarios,
        /// improving performance by avoiding the need to track changes.
        /// </remarks>
        public async Task<IEnumerable<TEntity>> GetAllAsync(bool asNoTracking = true, CancellationToken cancellationToken = default) =>
            await ProcessGetAll(asNoTracking, cancellationToken);

        /// <summary>
        /// Asynchronously retrieves the first entity that matches the given predicate.
        /// </summary>
        /// <param name="predicate">The condition to match the entity against.</param>
        /// <param name="asNoTracking">Specifies whether to disable entity tracking for performance improvements. Defaults to <c>true</c>.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A task representing the asynchronous operation, with a result of the first matching entity, or <c>null</c> if not found.</returns>
        /// <remarks>
        /// This method retrieves the first entity that satisfies the provided condition asynchronously.
        /// If no matching entity is found, <c>null</c> will be returned.
        /// </remarks>
        public async Task<TEntity?> GetFirstAsync(Expression<Func<TEntity, bool>> predicate, bool asNoTracking = true, CancellationToken cancellationToken = default) =>
            await ProcessGetFirst(predicate, asNoTracking, cancellationToken);

        /// <summary>
        /// Asynchronously retrieves all entities that match the given predicate.
        /// </summary>
        /// <param name="predicate">The condition to match the entities against.</param>
        /// <param name="asNoTracking">Specifies whether to disable entity tracking for performance improvements. Defaults to <c>true</c>.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A task representing the asynchronous operation, with a result of the matching entities.</returns>
        /// <remarks>
        /// This method retrieves all entities that satisfy the provided condition asynchronously.
        /// If no matching entities are found, an empty collection will be returned.
        /// </remarks>
        public async Task<IEnumerable<TEntity>> GetWhereAsync(Expression<Func<TEntity, bool>> predicate, bool asNoTracking = true, CancellationToken cancellationToken = default) =>
            await ProcessGetWhere(predicate, asNoTracking, cancellationToken);

        /// <summary>
        /// Asynchronously retrieves the first entity that matches the given condition and includes related entities.
        /// </summary>
        /// <typeparam name="TProperty">The type of the related entity to include.</typeparam>
        /// <param name="condition">The condition to match the entity against.</param>
        /// <param name="include">The related entity to include in the query.</param>
        /// <param name="asNoTracking">Specifies whether to disable entity tracking for performance improvements. Defaults to <c>true</c>.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A task representing the asynchronous operation, with a result of the first matching entity with related data, or <c>null</c> if not found.</returns>
        /// <remarks>
        /// This method retrieves the first entity that satisfies the provided condition asynchronously, along with its related data.
        /// If no matching entity is found, <c>null</c> will be returned.
        /// </remarks>
        public async Task<TEntity?> GetFirstWithRelatedAsync<TProperty>(Expression<Func<TEntity, bool>> condition, Expression<Func<TEntity, TProperty>> include, bool asNoTracking = true, CancellationToken cancellationToken = default) =>
            await ProcessGetFirstWithRelated(condition, include, asNoTracking, cancellationToken);

        /// <summary>
        /// Asynchronously retrieves all entities that match the given condition and includes related entities.
        /// </summary>
        /// <typeparam name="TProperty">The type of the related entity to include.</typeparam>
        /// <param name="condition">The condition to match the entities against.</param>
        /// <param name="include">The related entity to include in the query.</param>
        /// <param name="asNoTracking">Specifies whether to disable entity tracking for performance improvements. Defaults to <c>true</c>.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A task representing the asynchronous operation, with a result of the matching entities along with their related data.</returns>
        /// <remarks>
        /// This method retrieves all entities that satisfy the provided condition asynchronously, along with their related data.
        /// If no matching entities are found, an empty collection will be returned.
        /// </remarks>
        public async Task<IEnumerable<TEntity>> GetWithRelatedAsync<TProperty>(Expression<Func<TEntity, bool>> condition, Expression<Func<TEntity, TProperty>> include, bool asNoTracking = true, CancellationToken cancellationToken = default) =>
            await ProcessGetWithRelated(condition, include, asNoTracking, cancellationToken);

        /// <summary>
        /// Asynchronously checks if any entity matches the given condition.
        /// </summary>
        /// <param name="predicate">The condition to check against.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A task representing the asynchronous operation, with a result indicating whether an entity matching the condition exists.</returns>
        /// <remarks>
        /// This method checks if at least one entity satisfies the provided condition asynchronously.
        /// </remarks>
        public async Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default) =>
            await ProcessExists(predicate, cancellationToken);

        /// <summary>
        /// Asynchronously checks if there are any entities in the collection.
        /// </summary>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A task representing the asynchronous operation, with a result indicating whether the collection is empty.</returns>
        /// <remarks>
        /// This method checks if there are any entities in the collection asynchronously.
        /// </remarks>
        public async Task<bool> IsEmptyAsync(CancellationToken cancellationToken = default) =>
            await ProcessIsEmpty(cancellationToken);

        /// <summary>
        /// Asynchronously counts the total number of entities in the collection.
        /// </summary>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A task representing the asynchronous operation, with a result of the total number of entities in the collection.</returns>
        /// <remarks>
        /// This method counts the total number of entities asynchronously in the collection.
        /// </remarks>
        public async Task<int> CountAsync(CancellationToken cancellationToken = default) =>
            await ProcessCount(cancellationToken);

        /// <summary>
        /// Asynchronously counts the number of entities that match the given condition.
        /// </summary>
        /// <param name="predicate">The condition to match the entities against.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A task representing the asynchronous operation, with a result of the number of entities that match the condition.</returns>
        /// <remarks>
        /// This method counts the number of entities asynchronously that satisfy the provided condition.
        /// </remarks>
        public async Task<int> CountWhereAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default) =>
            await ProcessCountWhere(predicate, cancellationToken);

        /// <summary>
        /// Asynchronously calculates the maximum value of a specified field across all entities.
        /// </summary>
        /// <param name="selector">The selector to specify which field to calculate the maximum for.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A task representing the asynchronous operation, with a result of the maximum value of the specified field.</returns>
        /// <remarks>
        /// This method calculates the maximum value of a field asynchronously for all entities.
        /// </remarks>
        public async Task<decimal> MaxAsync(Expression<Func<TEntity, decimal>> selector, CancellationToken cancellationToken = default) =>
            await ProcessMax(selector, cancellationToken);

        /// <summary>
        /// Asynchronously calculates the maximum value of a specified field for entities that match the given condition.
        /// </summary>
        /// <param name="predicate">The condition to match the entities against.</param>
        /// <param name="selector">The selector to specify which field to calculate the maximum for.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A task representing the asynchronous operation, with a result of the maximum value of the specified field for matching entities.</returns>
        /// <remarks>
        /// This method calculates the maximum value of a field asynchronously for entities that satisfy the provided condition.
        /// </remarks>
        public async Task<decimal> MaxWhereAsync(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, decimal>> selector, CancellationToken cancellationToken = default) =>
            await ProcessMaxWhere(predicate, selector, cancellationToken);

        /// <summary>
        /// Asynchronously calculates the minimum value of a specified field across all entities.
        /// </summary>
        /// <param name="selector">The selector to specify which field to calculate the minimum for.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A task representing the asynchronous operation, with a result of the minimum value of the specified field.</returns>
        /// <remarks>
        /// This method calculates the minimum value of a field asynchronously for all entities.
        /// </remarks>
        public async Task<decimal> MinAsync(Expression<Func<TEntity, decimal>> selector, CancellationToken cancellationToken = default) =>
            await ProcessMin(selector, cancellationToken);

        /// <summary>
        /// Asynchronously calculates the minimum value of a specified field for entities that match the given condition.
        /// </summary>
        /// <param name="predicate">The condition to match the entities against.</param>
        /// <param name="selector">The selector to specify which field to calculate the minimum for.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A task representing the asynchronous operation, with a result of the minimum value of the specified field for matching entities.</returns>
        /// <remarks>
        /// This method calculates the minimum value of a field asynchronously for entities that satisfy the provided condition.
        /// </remarks>
        public async Task<decimal> MinWhereAsync(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, decimal>> selector, CancellationToken cancellationToken = default) =>
            await ProcessMinWhere(predicate, selector, cancellationToken);

        /// <summary>
        /// Asynchronously calculates the sum of a specified field across all entities.
        /// </summary>
        /// <param name="selector">The selector to specify which field to calculate the sum for.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A task representing the asynchronous operation, with a result of the sum of the specified field.</returns>
        /// <remarks>
        /// This method calculates the sum of a field asynchronously for all entities.
        /// </remarks>
        public async Task<decimal> SumAsync(Expression<Func<TEntity, decimal>> selector, CancellationToken cancellationToken = default) =>
            await ProcessSum(selector, cancellationToken);

        /// <summary>
        /// Asynchronously calculates the sum of a specified field for entities that match the given condition.
        /// </summary>
        /// <param name="predicate">The condition to match the entities against.</param>
        /// <param name="selector">The selector to specify which field to calculate the sum for.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A task representing the asynchronous operation, with a result of the sum of the specified field for matching entities.</returns>
        /// <remarks>
        /// This method calculates the sum of a field asynchronously for entities that satisfy the provided condition.
        /// </remarks>
        public async Task<decimal> SumWhereAsync(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, decimal>> selector, CancellationToken cancellationToken = default) =>
            await ProcessSumWhere(predicate, selector, cancellationToken);

        /// <summary>
        /// Asynchronously calculates the average value of a specified field across all entities.
        /// </summary>
        /// <param name="selector">The selector to specify which field to calculate the average for.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A task representing the asynchronous operation, with a result of the average value of the specified field.</returns>
        /// <remarks>
        /// This method calculates the average value of a field asynchronously for all entities.
        /// </remarks>
        public async Task<decimal> AverageAsync(Expression<Func<TEntity, decimal>> selector, CancellationToken cancellationToken = default) =>
            await ProcessAverage(selector, cancellationToken);

        /// <summary>
        /// Asynchronously calculates the average value of a specified field for entities that match the given condition.
        /// </summary>
        /// <param name="predicate">The condition to match the entities against.</param>
        /// <param name="selector">The selector to specify which field to calculate the average for.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A task representing the asynchronous operation, with a result of the average value of the specified field for matching entities.</returns>
        /// <remarks>
        /// This method calculates the average value of a field asynchronously for entities that satisfy the provided condition.
        /// </remarks>
        public async Task<decimal> AverageWhereAsync(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, decimal>> selector, CancellationToken cancellationToken = default) =>
            await ProcessAverageWhere(predicate, selector, cancellationToken);

        /// <summary>
        /// Asynchronously retrieves a sorted collection of entities based on the specified order.
        /// </summary>
        /// <param name="orderBy">The expression to determine how to order the entities.</param>
        /// <param name="isAscending">Indicates whether the order should be ascending or descending. Default is ascending.</param>
        /// <param name="asNoTracking">Indicates whether the query should be tracked by the context. Default is true (no tracking).</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A task representing the asynchronous operation, with a result of a sorted collection of entities.</returns>
        /// <remarks>
        /// This method retrieves the entities and orders them asynchronously based on the given order expression.
        /// </remarks>
        public async Task<IEnumerable<TEntity>> GetOrderedAsync(Expression<Func<TEntity, object>> orderBy, bool isAscending = true, bool asNoTracking = true, CancellationToken cancellationToken = default) =>
            await ProcessGetOrdered(orderBy, isAscending, asNoTracking, cancellationToken);

        /// <summary>
        /// Asynchronously retrieves a paginated collection of entities based on the specified page number and page size.
        /// </summary>
        /// <param name="pageNumber">The page number to retrieve.</param>
        /// <param name="pageSize">The number of entities per page.</param>
        /// <param name="orderBy">The expression to determine how to order the entities. Optional.</param>
        /// <param name="isAscending">Indicates whether the order should be ascending or descending. Default is ascending.</param>
        /// <param name="asNoTracking">Indicates whether the query should be tracked by the context. Default is true (no tracking).</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A task representing the asynchronous operation, with a result of a paginated collection of entities.</returns>
        /// <remarks>
        /// This method retrieves a page of entities based on the given page number and size, and orders them if an order expression is provided.
        /// </remarks>
        public async Task<IEnumerable<TEntity>> GetPagedAsync(int pageNumber, int pageSize, Expression<Func<TEntity, object>>? orderBy = null, bool isAscending = true, bool asNoTracking = true, CancellationToken cancellationToken = default) =>
            await ProcessGetPaged(pageNumber, pageSize, orderBy, isAscending, asNoTracking, cancellationToken);

        /// <summary>
        /// Asynchronously adds a new entity to the database.
        /// </summary>
        /// <param name="entityToAdd">The entity to be added.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        /// <remarks>
        /// This method asynchronously adds a new entity to the database and ensures that any required context or state changes are committed.
        /// </remarks>
        public async Task AddAsync(TEntity entityToAdd, CancellationToken cancellationToken = default) =>
            await ProcessAdd(entityToAdd, cancellationToken);

        /// <summary>
        /// Asynchronously adds a range of entities to the database.
        /// </summary>
        /// <param name="entitiesToAdd">The entities to be added.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        /// <remarks>
        /// This method asynchronously adds multiple entities to the database in one operation, which can be more efficient than adding them individually.
        /// </remarks>
        public async Task AddRangeAsync(IEnumerable<TEntity> entitiesToAdd, CancellationToken cancellationToken = default) =>
            await ProcessAddRange(entitiesToAdd, cancellationToken);

        /// <summary>
        /// Asynchronously updates an existing entity in the database.
        /// </summary>
        /// <param name="entityToUpdate">The entity to be updated.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        /// <remarks>
        /// This method asynchronously updates the specified entity, ensuring any changes are committed to the database.
        /// </remarks>
        public async Task UpdateAsync(TEntity entityToUpdate, CancellationToken cancellationToken = default) =>
            await ProcessUpdate(entityToUpdate, cancellationToken);

        /// <summary>
        /// Asynchronously updates an entity by its ID in the database using a specified update action.
        /// </summary>
        /// <param name="id">The ID of the entity to update.</param>
        /// <param name="updateAction">The action to update the entity's properties.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        /// <remarks>
        /// This method asynchronously finds the entity by its ID and applies the specified update action.
        /// </remarks>
        public async Task UpdateByIdAsync(int id, Action<TEntity> updateAction, CancellationToken cancellationToken = default) =>
            await ProcessUpdateById(id, updateAction, cancellationToken);

        /// <summary>
        /// Asynchronously updates a range of entities in the database.
        /// </summary>
        /// <param name="entitiesToUpdate">The entities to be updated.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        /// <remarks>
        /// This method asynchronously updates multiple entities in the database at once.
        /// </remarks>
        public async Task UpdateRangeAsync(IEnumerable<TEntity> entitiesToUpdate, CancellationToken cancellationToken = default) =>
            await ProcessUpdateRange(entitiesToUpdate, cancellationToken);

        /// <summary>
        /// Asynchronously updates entities that match a specified condition in the database.
        /// </summary>
        /// <param name="predicate">The condition to match entities.</param>
        /// <param name="updateAction">The action to update the matching entities.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        /// <remarks>
        /// This method asynchronously updates entities that satisfy the provided predicate, applying the update action to each matching entity.
        /// </remarks>
        public async Task UpdateWhereAsync(Func<TEntity, bool> predicate, Action<TEntity> updateAction, CancellationToken cancellationToken = default) =>
            await ProcessUpdateWhere(predicate, updateAction, cancellationToken);

        /// <summary>
        /// Asynchronously removes an entity from the database.
        /// </summary>
        /// <param name="entityToRemove">The entity to be removed.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        /// <remarks>
        /// This method asynchronously removes the specified entity from the database.
        /// </remarks>
        public async Task RemoveAsync(TEntity entityToRemove, CancellationToken cancellationToken = default) =>
            await ProcessRemove(entityToRemove, cancellationToken);

        /// <summary>
        /// Asynchronously removes an entity from the database by its ID.
        /// </summary>
        /// <param name="id">The ID of the entity to be removed.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        /// <remarks>
        /// This method asynchronously removes the entity identified by its ID from the database.
        /// </remarks>
        public async Task RemoveByIdAsync(int id, CancellationToken cancellationToken = default) =>
            await ProcessRemoveById(id, cancellationToken);

        /// <summary>
        /// Asynchronously removes a range of entities from the database.
        /// </summary>
        /// <param name="entitiesToRemove">The entities to be removed.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        /// <remarks>
        /// This method asynchronously removes multiple entities from the database.
        /// </remarks>
        public async Task RemoveRangeAsync(IEnumerable<TEntity> entitiesToRemove, CancellationToken cancellationToken = default) =>
            await ProcessRemoveRange(entitiesToRemove, cancellationToken);

        /// <summary>
        /// Asynchronously removes entities that match a specified condition from the database.
        /// </summary>
        /// <param name="predicate">The condition to match entities.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        /// <remarks>
        /// This method asynchronously removes entities that satisfy the provided predicate.
        /// </remarks>
        public async Task RemoveWhereAsync(Func<TEntity, bool> predicate, CancellationToken cancellationToken = default) =>
            await ProcessRemoveWhere(predicate, cancellationToken);

        /// <summary>
        /// Asynchronously executes a transaction with the specified action.
        /// </summary>
        /// <param name="action">The action to be executed inside the transaction.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        /// <remarks>
        /// This method asynchronously executes a transaction, executing the specified action within the scope of the transaction.
        /// </remarks>
        public async Task ExecuteTransactionAsync(Func<Task> action, CancellationToken cancellationToken = default) =>
            await ProcessTransactExecution(action, cancellationToken);

        /// <summary>
        /// Asynchronously executes a raw SQL query and returns the result as an enumerable of entities.
        /// </summary>
        /// <param name="sqlQuery">The raw SQL query to execute.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A task representing the asynchronous operation with the result of the query.</returns>
        /// <remarks>
        /// This method executes a raw SQL query asynchronously and returns the result as a collection of entities.
        /// </remarks>
        public async Task<IEnumerable<TEntity>> ExecuteSqlRawAsync(string sqlQuery, CancellationToken cancellationToken = default) =>
            await ProcessSqlRawExecution(sqlQuery, cancellationToken);

        /// <summary>
        /// Asynchronously executes a raw SQL command and returns the number of rows affected.
        /// </summary>
        /// <param name="sqlQuery">The raw SQL command to execute.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A task representing the asynchronous operation, with the number of rows affected.</returns>
        /// <remarks>
        /// This method asynchronously executes a raw SQL command (e.g., an insert, update, or delete operation) and returns
        /// the number of rows affected by the command. It is useful when executing commands that modify the data in the database.
        /// </remarks>
        public async Task<int> ExecuteSqlRawCommandAsync(string sqlQuery, CancellationToken cancellationToken = default) =>
            await ProcessSqlRawCommandExecution(sqlQuery, cancellationToken);

        #endregion
    }
}
