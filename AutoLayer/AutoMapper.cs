using AutoLayer.Exceptions;
using AutoLayer.Resources;

namespace AutoLayer
{
    public static class AutoMapper
    {
        public static TModel MapToModel<TModel>(object entity) where TModel : class, new()
        {
            var modelType = typeof(TModel);
            var entityType = entity.GetType();

            var modelLength = modelType.GetProperties().Length;
            var entityLength = entityType.GetProperties().Length;

            if (modelLength != entityLength)
                throw new MapException(Error.EntityToModelError, entity.GetType().Name, modelType.Name);

            for (int i = 0; i < modelLength; i++)           
                modelType.GetProperties()[i].SetValue(entity, entityType.GetProperties()[i].GetValue(entity));

            return (TModel)(object)modelType;
        }

        public static TEntity MapToEntity<TEntity>(object model) where TEntity : class, new()
        {
            var entityType = typeof(TEntity);
            var modelType = model.GetType();

            var entityLength = entityType.GetProperties().Length;
            var modelLength = modelType.GetProperties().Length;

            if (entityLength != modelLength)
                throw new MapException(Error.ModelToEntityError, modelType.GetType().Name, entityType.Name);

            for (int i = 0; i < entityLength; i++)           
                entityType.GetProperties()[i].SetValue(model, modelType.GetProperties()[i].GetValue(model));

            return (TEntity)(object)entityType;
        }
    }
}
