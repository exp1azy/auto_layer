namespace AutoLayer
{
    public interface IRepository<TEntity> where TEntity : class
    {
        public void Add(TEntity entityToAdd);
        public void Update(TEntity updatedEntity);
        public void Remove(TEntity entityToRemove);
        public TEntity GetById(int id);
        public IEnumerable<TEntity> GetAll();
        public IEnumerable<TEntity> Find(Func<TEntity, bool> predicate);
        public void ExecuteTransaction(Action action);
    }
}
