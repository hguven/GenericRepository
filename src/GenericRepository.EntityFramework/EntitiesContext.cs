using System.Data;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Data.Linq;

namespace GenericRepository.EntityFramework {

    /// <summary>
    /// Thin wrapper around the DbContext.
    /// </summary>
    public abstract class EntitiesContext : DbContext, IEntitiesContext
    {

     
        /// <summary>
        /// Constructs a new context instance using the given string as the name or connection
        /// string for the database to which a connection will be made.  See the class
        /// remarks for how this is used to create a connection.
        /// </summary>
        /// <param name="nameOrConnectionString">Either the database name or a connection string.</param>
        protected EntitiesContext(string nameOrConnectionString) : base(nameOrConnectionString) { 

         
        }
         
        /// <summary>
        /// Returns a DbSet instance for access to entities of the given type in the context.
        /// </summary>
        /// <remarks>
        /// This method calls the DbContext.Set method.
        /// </remarks>
        /// <typeparam name="TEntity">The type entity for which a set should be returned.</typeparam>
        /// <returns>A set for the given entity type.</returns>
        public new IDbSet<TEntity> Set<TEntity>() where TEntity : class {
          
            return base.Set<TEntity>();
        }

        /// <summary>
        /// Sets the entity state as <see cref="EntityState.Added"/>.
        /// </summary>
        /// <typeparam name="TEntity">Type of the entity</typeparam>
        /// <param name="entity">The entity whose state needs to be set as <see cref="EntityState.Added"/>.</param>
        public void SetAsAdded<TEntity>(TEntity entity) where TEntity : class {

            DbEntityEntry dbEntityEntry = GetDbEntityEntrySafely(entity);
            dbEntityEntry.State = EntityState.Added;
        }

        /// <summary>
        /// Sets the entity state as <see cref="EntityState.Modified"/>.
        /// </summary>
        /// <typeparam name="TEntity">Type of the entity</typeparam>
        /// <param name="entity">The entity whose state needs to be set as <see cref="EntityState.Modified"/>.</param>
        public void SetAsModified<TEntity>(TEntity entity) where TEntity : class {

            DbEntityEntry dbEntityEntry = GetDbEntityEntrySafely(entity);
            dbEntityEntry.State = EntityState.Modified;
        }

        /// <summary>
        /// Sets the entity state as <see cref="EntityState.Deleted"/>.
        /// </summary>
        /// <typeparam name="TEntity">Type of the entity</typeparam>
        /// <param name="entity">The entity whose state needs to be set as <see cref="EntityState.Deleted"/>.</param>
        public void SetAsDeleted<TEntity>(TEntity entity) where TEntity : class {

            DbEntityEntry dbEntityEntry = GetDbEntityEntrySafely(entity);
            dbEntityEntry.State = EntityState.Deleted;
        }

         


        // privates
        private DbEntityEntry GetDbEntityEntrySafely<TEntity>(TEntity entity) where TEntity : class {

            DbEntityEntry dbEntityEntry = base.Entry<TEntity>(entity);
            if (dbEntityEntry.State == EntityState.Detached) {

                Set<TEntity>().Attach(entity);
            }

            return dbEntityEntry;
        }
    }
}