using AutoLayer.Exceptions;
using AutoLayer.Resources;

namespace AutoLayer
{
    /// <summary>
    /// A static utility class for mapping between entities and models.
    /// Provides methods for transforming objects from one type to another.
    /// </summary>
    public static class AutoMapper
    {
        /// <summary>
        /// Maps an entity to a model of type <typeparamref name="TModel"/>.
        /// </summary>
        /// <typeparam name="TModel">The type of the model to map to.</typeparam>
        /// <param name="entity">The entity object to map from.</param>
        /// <returns>A mapped instance of type <typeparamref name="TModel"/>.</returns>
        /// <exception cref="NullEntityException">Thrown if the <paramref name="entity"/> is null.</exception>
        /// <exception cref="MapException">Thrown if mapping fails due to property type mismatches.</exception>
        public static TModel MapToModel<TModel>(object entity) where TModel : class, new() =>
            FromEntityToModel<TModel>(entity);

        /// <summary>
        /// Maps a model to an entity of type <typeparamref name="TEntity"/>.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity to map to.</typeparam>
        /// <param name="model">The model object to map from.</param>
        /// <returns>A mapped instance of type <typeparamref name="TEntity"/>.</returns>
        /// <exception cref="NullEntityException">Thrown if the <paramref name="model"/> is null.</exception>
        /// <exception cref="MapException">Thrown if mapping fails due to property type mismatches.</exception>
        public static TEntity MapToEntity<TEntity>(object model) where TEntity : class, new() =>
            FromModelToEntity<TEntity>(model);

        /// <summary>
        /// Maps a list of entities to a list of models of type <typeparamref name="TModel"/>.
        /// </summary>
        /// <typeparam name="TModel">The type of the model to map to.</typeparam>
        /// <param name="entities">The list of entities to map from.</param>
        /// <returns>A list of mapped models of type <typeparamref name="TModel"/>.</returns>
        public static List<TModel> MapToModelList<TModel>(List<object> entities) where TModel : class, new() =>
            entities.Select(FromEntityToModel<TModel>).ToList();

        /// <summary>
        /// Maps a list of models to a list of entities of type <typeparamref name="TEntity"/>.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity to map to.</typeparam>
        /// <param name="models">The list of models to map from.</param>
        /// <returns>A list of mapped entities of type <typeparamref name="TEntity"/>.</returns>
        public static List<TEntity> MapToEntityList<TEntity>(List<object> models) where TEntity : class, new() =>
            models.Select(FromModelToEntity<TEntity>).ToList();

        /// <summary>
        /// Converts an entity object to a model of type <typeparamref name="TModel"/>.
        /// </summary>
        /// <typeparam name="TModel">The type of the model to map to.</typeparam>
        /// <param name="entity">The entity object to map from.</param>
        /// <returns>A mapped instance of type <typeparamref name="TModel"/>.</returns>
        /// <exception cref="NullEntityException">Thrown if the <paramref name="entity"/> is null.</exception>
        /// <exception cref="MapException">Thrown if mapping fails due to property type mismatches or missing properties.</exception>
        private static TModel FromEntityToModel<TModel>(object entity) where TModel : class, new()
        {
            if (entity == null)
                throw new NullEntityException(Error.NullEntityError, nameof(entity));

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

        /// <summary>
        /// Converts a model object to an entity of type <typeparamref name="TEntity"/>.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity to map to.</typeparam>
        /// <param name="model">The model object to map from.</param>
        /// <returns>A mapped instance of type <typeparamref name="TEntity"/>.</returns>
        /// <exception cref="NullEntityException">Thrown if the <paramref name="model"/> is null.</exception>
        /// <exception cref="MapException">Thrown if mapping fails due to property type mismatches or missing properties.</exception>
        private static TEntity FromModelToEntity<TEntity>(object model) where TEntity : class, new()
        {
            if (model == null)
                throw new NullEntityException(Error.NullEntityError, nameof(model));

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
