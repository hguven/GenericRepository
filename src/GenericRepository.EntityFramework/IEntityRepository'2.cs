using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Linq;
using System.Threading.Tasks;
// Entity Framework 6 to support async methods
namespace GenericRepository.EntityFramework
{

    /// <summary>
    /// Entity Framework interface implementation for IRepository.
    /// </summary>
    /// <typeparam name="TEntity">Type of entity</typeparam>
    /// <typeparam name="TId">Type of entity Id</typeparam>
    public interface IEntityRepository<TEntity, TId> : IRepository<TEntity, TId>
        where TEntity : class, IEntity<TId>
        where TId : IComparable
    {

        IQueryable<TEntity> GetAllIncluding(params Expression<Func<TEntity, object>>[] includeProperties);
        TEntity GetSingleIncluding(TId id, params Expression<Func<TEntity, object>>[] includeProperties);

        PaginatedList<TEntity> Paginate<TKey>(
            int pageIndex, int pageSize, Expression<Func<TEntity, TKey>> keySelector);

        PaginatedList<TEntity> Paginate<TKey>(
            int pageIndex, int pageSize, Expression<Func<TEntity, TKey>> keySelector, Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] includeProperties);

        PaginatedList<TEntity> PaginateDescending<TKey>(
            int pageIndex, int pageSize, Expression<Func<TEntity, TKey>> keySelector);

        PaginatedList<TEntity> PaginateDescending<TKey>(
            int pageIndex, int pageSize, Expression<Func<TEntity, TKey>> keySelector, Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] includeProperties);

        void Add(TEntity entity);
        void AddGraph(TEntity entity);
        void Edit(TEntity entity);
        void Delete(TEntity entity);
        int Save();
        Task<int> DeleteAsync(TEntity entity);
        Task<ICollection<TEntity>> GetAllAsync();
        Task<TEntity> GetAsync(TId id);
        Task<TEntity> FindAsync(Expression<Func<TEntity, bool>> match);
        Task<ICollection<TEntity>> FindAllAsync(Expression<Func<TEntity, bool>> match);
        Task<TEntity> AddAsync(TEntity t);
        Task<TEntity> EditAsync(TEntity updated, TId id);
        int Count();
        Task<int> CountAsync();


    }
}