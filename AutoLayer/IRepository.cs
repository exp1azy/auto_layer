using System.Linq.Expressions;

namespace AutoLayer
{
    public interface IRepository<TEntity> where TEntity : class, new()
    {
        #region Sync
        public TEntity? GetById(int id);
        public IEnumerable<TEntity> GetAll(bool asNoTracking = true);
        public IEnumerable<TEntity> GetWhere(Func<TEntity, bool> predicate, bool asNoTracking = true);
        public bool Exists(Func<TEntity, bool> predicate);
        public int Count();
        public int CountWhere(Func<TEntity, bool> predicate);
        public IQueryable<TEntity> GetQuery();
        public IEnumerable<TEntity> GetOrdered(Func<TEntity, bool> orderBy, bool isAscending = true, bool asNoTracking = true);
        public IEnumerable<TEntity> GetPaged(int pageNumber, int pageSize, Func<TEntity, bool>? orderBy = null, bool isAscending = true, bool asNoTracking = true);
        public void Add(TEntity entityToAdd);
        public void AddRange(IEnumerable<TEntity> entitiesToAdd);
        public void Update(TEntity entityToUpdate);
        public void UpdateById(int id, Action<TEntity> updateAction);
        public void UpdateRange(IEnumerable<TEntity> entitiesToUpdate);
        public void UpdateWhere(Func<TEntity, bool> predicate, Action<TEntity> updateAction);
        public void Remove(TEntity entityToRemove);
        public void RemoveById(int id);
        public void RemoveRange(IEnumerable<TEntity> entitiesToRemove);
        public void RemoveWhere(Func<TEntity, bool> predicate);
        public void ExecuteTransaction(Action action);
        public IEnumerable<TEntity> ExecuteSqlRaw(string sqlQuery);
        public int ExecuteSqlRawCommand(string sqlQuery);
        #endregion

        #region Async
        public Task<TEntity?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        public Task<IEnumerable<TEntity>> GetAllAsync(bool asNoTracking = true, CancellationToken cancellationToken = default);
        public Task<IEnumerable<TEntity>> GetWhereAsync(Func<TEntity, bool> predicate, bool asNoTracking = true, CancellationToken cancellationToken = default);
        public Task<bool> ExistsAsync(Func<TEntity, bool> predicate, CancellationToken cancellationToken = default);
        public Task<int> CountAsync(CancellationToken cancellationToken = default);
        public Task<int> CountWhereAsync(Func<TEntity, bool> predicate, CancellationToken cancellationToken = default);
        public Task<IQueryable<TEntity>> GetQueryAsync(CancellationToken cancellationToken = default);
        public Task<IEnumerable<TEntity>> GetOrderedAsync(Func<TEntity, bool> orderBy, bool isAscending = true, bool asNoTracking = true, CancellationToken cancellationToken = default);
        public Task<IEnumerable<TEntity>> GetPagedAsync(int pageNumber, int pageSize, Func<TEntity, bool>? orderBy = null, bool isAscending = true, bool asNoTracking = true, CancellationToken cancellationToken = default);
        public Task AddAsync(TEntity entityToAdd, CancellationToken cancellationToken = default);
        public Task AddRangeAsync(IEnumerable<TEntity> entitiesToAdd, CancellationToken cancellationToken = default);
        public Task UpdateAsync(TEntity updatedEntity, CancellationToken cancellationToken = default);
        public Task UpdateByIdAsync(int id, Action<TEntity> updateAction, CancellationToken cancellationToken = default);
        public Task UpdateRangeAsync(IEnumerable<TEntity> entitiesToUpdate, CancellationToken cancellationToken = default);
        public Task UpdateWhereAsync(Func<TEntity, bool> predicate, Action<TEntity> updateAction, CancellationToken cancellationToken = default);
        public Task RemoveAsync(TEntity entityToRemove, CancellationToken cancellationToken = default);
        public Task RemoveByIdAsync(int id, CancellationToken cancellationToken = default);
        public Task RemoveRangeAsync(IEnumerable<TEntity> entitiesToRemove, CancellationToken cancellationToken = default);
        public Task RemoveWhereAsync(Func<TEntity, bool> predicate, CancellationToken cancellationToken = default);
        public Task ExecuteTransactionAsync(Func<Task> action, CancellationToken cancellationToken = default);
        public Task<IEnumerable<TEntity>> ExecuteSqlRawAsync(string sqlQuery, CancellationToken cancellationToken = default);
        public Task<int> ExecuteSqlRawCommandAsync(string sqlQuery, CancellationToken cancellationToken = default);

        #endregion
    }
}
