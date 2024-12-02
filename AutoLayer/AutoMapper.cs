using AutoLayer.Exceptions;
using AutoLayer.Resources;

namespace AutoLayer
{
    public static class AutoMapper
    {
        public static TModel MapToModel<TModel>(object entity) where TModel : class, new()
        {
            if (entity == null)
                throw new NullEntityException(nameof(entity));

            var model = new TModel();
            var modelProperties = typeof(TModel).GetProperties();
            var entityProperties = entity.GetType().GetProperties();

            foreach (var modelProperty in modelProperties)
            {
                var entityProperty = entityProperties.FirstOrDefault(p => p.Name == modelProperty.Name) 
                    ?? throw new MapException(Error.EntityToModelError, entity.GetType().Name, typeof(TModel).Name);

                if (modelProperty.PropertyType.IsAssignableFrom(entityProperty.PropertyType))
                {
                    var value = entityProperty.GetValue(entity);
                    modelProperty.SetValue(model, value);
                }
                else
                {
                    throw new MapException(Error.EntityToModelError, entityProperty.Name, entityProperty.PropertyType.Name, modelProperty.PropertyType.Name);
                }
            }

            return model;
        }

        public static TEntity MapToEntity<TEntity>(object model) where TEntity : class, new()
        {
            if (model == null)
                throw new NullEntityException(nameof(model));

            var entity = new TEntity();
            var entityProperties = typeof(TEntity).GetProperties();
            var modelProperties = model.GetType().GetProperties();

            foreach (var entityProperty in entityProperties)
            {
                var modelProperty = modelProperties.FirstOrDefault(p => p.Name == entityProperty.Name) 
                    ?? throw new MapException(Error.ModelToEntityError, model.GetType().Name, typeof(TEntity).Name);

                if (entityProperty.PropertyType.IsAssignableFrom(modelProperty.PropertyType))
                {
                    var value = modelProperty.GetValue(model);
                    entityProperty.SetValue(entity, value);
                }
                else
                {
                    throw new MapException(Error.ModelToEntityError, modelProperty.Name, modelProperty.PropertyType.Name, entityProperty.PropertyType.Name);
                }
            }

            return entity;
        }
    }
}
