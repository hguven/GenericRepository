using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq.Expressions;
using System.Linq;
using System.Threading.Tasks;
using GenericRepository.EntityFramework.Enums;

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
        Task<List<TEntity>> GetAllAsync();
        Task<TEntity> FindAsync(Expression<Func<TEntity, bool>> match);
        Task<List<TEntity>> FindAllAsync<TKey>(Expression<Func<TEntity, bool>> match, Expression<Func<TEntity, TKey>> keySelector, OrderByType orderByType, int? take, int ? skip);

        List<TEntity> FindAll<TKey>(Expression<Func<TEntity, bool>> match, Expression<Func<TEntity, TKey>> keySelector,
                                    OrderByType orderByType, int? take, int? skip);
        Task<List<TEntity>> FindAllAsync<TKey>(Expression<Func<TEntity, bool>> match, Expression<Func<TEntity, TKey>> keySelector, OrderByType orderByType, int page, int pageSize);
        Task<TEntity> AddAsync(TEntity t);
        Task<TEntity> EditAsync(TEntity updated, TId id);
        Task<List<TEntity>> GetAllIncludingAsync<TKey>(int? take, int ? skip, Expression<Func<TEntity, TKey>> keySelector, OrderByType orderByType, params Expression<Func<TEntity, object>>[] includeProperties);
        Task<List<TEntity>> GetAllIncludingAsync<TKey>(int page, int pageSize, Expression<Func<TEntity, TKey>> keySelector, OrderByType orderByType, params Expression<Func<TEntity, object>>[] includeProperties);
        Task<TEntity> GetSingleAsync(TId id);

        Task<TEntity> GetFirstAsync<TKey>(Expression<Func<TEntity, bool>> match, Expression<Func<TEntity, TKey>> keySelector, OrderByType orderByType);
        Task<TEntity> GetFirstAsync<TKey>(Expression<Func<TEntity, TKey>> keySelector, OrderByType orderByType);
 


        Task<TEntity> GetSingleIncludingAsync(TId id,  params Expression<Func<TEntity, object>>[] includeProperties);
        int Count();
        Task<int> CountAsync();
        int Count(Expression<Func<TEntity, bool>> match);
        Task<int> CountAsync(Expression<Func<TEntity, bool>> match);
        Task<List<TEntity>> FindAllIncludingAsync<TKey>(Expression<Func<TEntity, bool>> match, int? take,int ? skip, Expression<Func<TEntity, TKey>> keySelector, OrderByType orderByType, params Expression<Func<TEntity, object>>[] includeProperties);
        Task<List<TEntity>> FindAllIncludingAsync<TKey>(Expression<Func<TEntity, bool>> match, int page, int pageSize, Expression<Func<TEntity, TKey>> keySelector,  OrderByType orderByType, params Expression<Func<TEntity, object>>[] includeProperties);

        bool Contains(Expression<Func<TEntity, bool>> predicate);
        void Delete(Expression<Func<TEntity, bool>> predicate);

        /// <summary>
        /// Executes the procedure.
        /// </summary>
        /// <param name="procedureCommand">The procedure command.</param>
        /// <param name="sqlParams">The SQL params.</param>
        void ExecuteProcedure(String procedureCommand, params SqlParameter[] sqlParams);


    }
}