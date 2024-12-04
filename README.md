# AutoLayer

AutoLayer is a powerful and flexible .NET library that simplifies and standardizes data access by replacing the repository layer. 
It supports CRUD operations, advanced querying, transactions, and more. With a focus on performance, scalability, and ease of use, AutoLayer is perfect for enterprise applications that need a clean and maintainable architecture.

## Key Features

- **Repository Pattern**: Implements the repository pattern to simplify data access and ensure separation of concerns.
- **CRUD Operations**: Perform Create, Read, Update, and Delete operations easily on any entity.
- **Async Support**: Fully supports asynchronous operations for improved performance.
- **Transactions**: Manage transactions to ensure atomicity and consistency in database operations.
- **SQL Execution**: Execute SQL queries if needed.
- **Batch Operations**: Efficiently perform bulk operations, like adding, updating, or deleting multiple entities at once.
- **LINQ Support**: Use LINQ queries to filter, order, and group your data with ease.
- **Entity Mapping**: Provides utility methods for mapping between entities and models.
- **Error Handling**: Graceful error handling for common data access issues.

## Installation

You can install AutoLayer via NuGet:

```bash
Install-Package AutoLayer
```

Or via .NET CLI:

```bash
dotnet add package AutoLayer
```

## Usage

### Basic Setup
You can initialize the repository directly in your service by passing it the database context as a parameter.

```csharp
public class UserService
{
    private readonly Repository<User> _userRepository;

    public UserService(MyDbContext context)
    {
        _userRepository = new(context);
    }
}
```

You can also create your own repository by inheriting from Repository<TEntity> if you need to define and implement your own logic.

```csharp
public class UserRepository : Repository<User>
{
    public UserRepository(MyDbContext context) : base(context)
    {
    }
}
```

### CRUD Operations

```csharp
// Add entity
var entity = new MyEntity { Name = "Example" };
await repository.AddAsync(entity);

// Get by ID
var entity = await repository.GetByIdAsync(1);

// Get all entities
var allEntities = await repository.GetAllAsync();

// Update entity
entity.Name = "Updated Name";
await repository.UpdateAsync(entity);

// Remove entity
await repository.RemoveAsync(entity);
```

### Advanced Querying
You can also use advanced querying features like filtering, sorting, and pagination:

```csharp
// Get entities with conditions
var filteredEntities = await repository.GetWhereAsync(e => e.Name.Contains("Example"));

// Order entities
var orderedEntities = await repository.GetOrderedAsync(e => e.Name, isAscending: true);

// Pagination
var pagedEntities = await repository.GetPagedAsync(pageNumber: 1, pageSize: 10);
```

### Transactions
You can wrap operations inside a transaction to ensure atomicity:

```csharp
await repository.ExecuteTransactionAsync(async () =>
{
    var entity1 = new MyEntity { Name = "Entity 1" };
    var entity2 = new MyEntity { Name = "Entity 2" };

    await repository.AddAsync(entity1);
    await repository.AddAsync(entity2);
});
```

### Methods Overview

```csharp
#region Sync
    public TEntity? GetById(int id);
    public IEnumerable<TEntity> GetAll(bool asNoTracking = true);
    public TEntity? GetFirst(Expression<Func<TEntity, bool>> predicate, bool asNoTracking = true);
    public IEnumerable<TEntity> GetWhere(Expression<Func<TEntity, bool>> predicate, bool asNoTracking = true);
    public TEntity? GetFirstWithRelated<TProperty>(Expression<Func<TEntity, bool>> condition, Expression<Func<TEntity, TProperty>> include, bool asNoTracking = true);
    public IEnumerable<TEntity> GetWithRelated<TProperty>(Expression<Func<TEntity, bool>> condition, Expression<Func<TEntity, TProperty>> include, bool asNoTracking = true);
    public bool Exists(Expression<Func<TEntity, bool>> predicate);
    public bool IsEmpty();
    public int Count();
    public int CountWhere(Expression<Func<TEntity, bool>> predicate);
    public decimal Max(Expression<Func<TEntity, decimal>> selector);
    public decimal MaxWhere(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, decimal>> selector);
    public decimal Min(Expression<Func<TEntity, decimal>> selector);
    public decimal MinWhere(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, decimal>> selector);
    public decimal Sum(Expression<Func<TEntity, decimal>> selector);
    public decimal SumWhere(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, decimal>> selector);
    public decimal Average(Expression<Func<TEntity, decimal>> selector);
    public decimal AverageWhere(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, decimal>> selector);
    public IQueryable<TEntity> GetQuery();
    public IEnumerable<TEntity> GetOrdered(Expression<Func<TEntity, object>> orderBy, bool isAscending = true, bool asNoTracking = true);
    public IEnumerable<TEntity> GetPaged(int pageNumber, int pageSize, Expression<Func<TEntity, object>>? orderBy = null, bool isAscending = true, bool asNoTracking = true);
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
    public void ExecuteTransaction(Func<Task> action);
    public IEnumerable<TEntity> ExecuteSqlRaw(string sqlQuery);
    public int ExecuteSqlRawCommand(string sqlQuery);
#endregion

#region Async
    public Task<TEntity?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    public Task<IEnumerable<TEntity>> GetAllAsync(bool asNoTracking = true, CancellationToken cancellationToken = default);
    public Task<TEntity?> GetFirstAsync(Expression<Func<TEntity, bool>> predicate, bool asNoTracking = true, CancellationToken cancellationToken = default);
    public Task<IEnumerable<TEntity>> GetWhereAsync(Expression<Func<TEntity, bool>> predicate, bool asNoTracking = true, CancellationToken cancellationToken = default);
    public Task<TEntity?> GetFirstWithRelatedAsync<TProperty>(Expression<Func<TEntity, bool>> condition, Expression<Func<TEntity, TProperty>> include, bool asNoTracking = true, CancellationToken cancellationToken = default);
    public Task<IEnumerable<TEntity>> GetWithRelatedAsync<TProperty>(Expression<Func<TEntity, bool>> condition, Expression<Func<TEntity, TProperty>> include, bool asNoTracking = true, CancellationToken cancellationToken = default);
    public Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);
    public Task<bool> IsEmptyAsync(CancellationToken cancellationToken = default);
    public Task<int> CountAsync(CancellationToken cancellationToken = default);
    public Task<int> CountWhereAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);
    public Task<decimal> MaxAsync(Expression<Func<TEntity, decimal>> selector, CancellationToken cancellationToken = default);
    public Task<decimal> MaxWhereAsync(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, decimal>> selector, CancellationToken cancellationToken = default);
    public Task<decimal> MinAsync(Expression<Func<TEntity, decimal>> selector, CancellationToken cancellationToken = default);
    public Task<decimal> MinWhereAsync(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, decimal>> selector, CancellationToken cancellationToken = default);
    public Task<decimal> SumAsync(Expression<Func<TEntity, decimal>> selector, CancellationToken cancellationToken = default);
    public Task<decimal> SumWhereAsync(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, decimal>> selector, CancellationToken cancellationToken = default);
    public Task<decimal> AverageAsync(Expression<Func<TEntity, decimal>> selector, CancellationToken cancellationToken = default);
    public Task<decimal> AverageWhereAsync(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, decimal>> selector, CancellationToken cancellationToken = default);
    public Task<IEnumerable<TEntity>> GetOrderedAsync(Expression<Func<TEntity, object>> orderBy, bool isAscending = true, bool asNoTracking = true, CancellationToken cancellationToken = default);
    public Task<IEnumerable<TEntity>> GetPagedAsync(int pageNumber, int pageSize, Expression<Func<TEntity, object>>? orderBy = null, bool isAscending = true, bool asNoTracking = true, CancellationToken cancellationToken = default);
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
```

## License

AutoLayer is licensed under the MIT License. See the [LICENSE](https://github.com/exp1azy/auto_layer/blob/main/AutoLayer/LICENSE.txt) file for more information.
