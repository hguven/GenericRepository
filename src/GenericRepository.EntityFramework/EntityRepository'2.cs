using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using GenericRepository.EntityFramework.Enums;

namespace GenericRepository.EntityFramework
{

    /// <summary>
    /// IEntityRepository implementation for DbContext instance.
    /// </summary>
    /// <typeparam name="TEntity">Type of entity</typeparam>
    /// <typeparam name="TId">Type of entity Id</typeparam>
    public class EntityRepository<TEntity, TId> : IEntityRepository<TEntity, TId>
        where TEntity : class, IEntity<TId>
        where TId : IComparable
    {

        private readonly IEntitiesContext _dbContext;
         

        public EntityRepository(IEntitiesContext dbContext)
        {

            if (dbContext == null)
            {

                throw new ArgumentNullException("dbContext");
            }

            _dbContext = dbContext;
        }


        public IQueryable<TEntity> GetAll()
        {

            return _dbContext.Set<TEntity>();
        }

        public IQueryable<TEntity> GetAllIncluding(params Expression<Func<TEntity, object>>[] includeProperties)
        {

            IQueryable<TEntity> queryable = GetAll();
            foreach (Expression<Func<TEntity, object>> includeProperty in includeProperties)
            {

                queryable = queryable.Include<TEntity, object>(includeProperty);
            }

            return queryable;
        }

        public IQueryable<TEntity> FindBy(Expression<Func<TEntity, bool>> predicate)
        {

            IQueryable<TEntity> queryable = GetAll().Where<TEntity>(predicate);
            return queryable;
        }

        public PaginatedList<TEntity> Paginate(int pageIndex, int pageSize)
        {

            PaginatedList<TEntity> paginatedList = Paginate<TId>(pageIndex, pageSize, x => x.Id);
            return paginatedList;
        }

        public PaginatedList<TEntity> Paginate<TKey>(int pageIndex, int pageSize, Expression<Func<TEntity, TKey>> keySelector)
        {

            return Paginate<TKey>(pageIndex, pageSize, keySelector, null);
        }

        public PaginatedList<TEntity> Paginate<TKey>(int pageIndex, int pageSize, Expression<Func<TEntity, TKey>> keySelector, Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] includeProperties)
        {

            PaginatedList<TEntity> paginatedList = Paginate<TKey>(
                pageIndex, pageSize, keySelector, predicate, OrderByType.Ascending, includeProperties);

            return paginatedList;
        }

        public PaginatedList<TEntity> PaginateDescending<TKey>(int pageIndex, int pageSize, Expression<Func<TEntity, TKey>> keySelector)
        {

            return PaginateDescending<TKey>(pageIndex, pageSize, keySelector, null);
        }

        public PaginatedList<TEntity> PaginateDescending<TKey>(int pageIndex, int pageSize, Expression<Func<TEntity, TKey>> keySelector, Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] includeProperties)
        {

            PaginatedList<TEntity> paginatedList = Paginate<TKey>(
                pageIndex, pageSize, keySelector, predicate, OrderByType.Descending, includeProperties);

            return paginatedList;
        }

        public TEntity GetSingle(TId id)
        {

            IQueryable<TEntity> entities = GetAll();
            TEntity entity = Filter<TId>(entities, x => x.Id, id).FirstOrDefault();
            return entity;
        }

        public TEntity GetSingleIncluding(TId id, params Expression<Func<TEntity, object>>[] includeProperties)
        {

            IQueryable<TEntity> entities = GetAllIncluding(includeProperties);
            TEntity entity = Filter<TId>(entities, x => x.Id, id).FirstOrDefault();
            return entity;
        }

        public void Add(TEntity entity)
        {

            _dbContext.SetAsAdded(entity);
        }

        public void AddGraph(TEntity entity)
        {

            _dbContext.Set<TEntity>().Add(entity);
        }

        public void Edit(TEntity entity)
        {

            _dbContext.SetAsModified(entity);
        }

        public void Delete(TEntity entity)
        {

            _dbContext.SetAsDeleted(entity);
        }

        public int Save()
        {

            return _dbContext.SaveChanges();
        }


        public async Task<int> DeleteAsync(TEntity t)
        {
            _dbContext.Set<TEntity>().Remove(t);
            return await _dbContext.SaveChangesAsync();
        }

        public async Task<List<TEntity>> GetAllAsync()
        {
            return await GetAll().ToListAsync();
        }


        public async Task<List<TEntity>> GetAllIncludingAsync<TKey>(int page, int pageSize, Expression<Func<TEntity, TKey>> keySelector, OrderByType orderByType,
                                               params Expression<Func<TEntity, object>>[] includeProperties)
        {
            var queryable = GetAll();
            queryable = (orderByType == OrderByType.Ascending) ? queryable.OrderBy(keySelector) : queryable.OrderByDescending(keySelector);
            foreach (Expression<Func<TEntity, object>> includeProperty in includeProperties)
            {
                queryable = queryable.Include<TEntity, object>(includeProperty);
            }

            if (page > 0)
            {
                if (page == 0) page = 1;
                int skip = (page - 1) * pageSize;
                queryable = queryable.Skip(skip).Take(pageSize);
            }
            return await queryable.ToListAsync();
        }

        public async Task<TEntity> GetSingleAsync(TId id)
        {
            IQueryable<TEntity> entities = GetAll();
            var entity = await Filter<TId>(entities, x => x.Id, id).FirstOrDefaultAsync();
            return entity;
        }

        public async Task<TEntity> GetFirstAsync<TKey>(Expression<Func<TEntity, bool>> match, Expression<Func<TEntity, TKey>> keySelector, OrderByType orderByType)
        {
            var queryable = FindBy(match);
            queryable = (orderByType == OrderByType.Ascending) ? queryable.OrderBy(keySelector) : queryable.OrderByDescending(keySelector);
            return await queryable.FirstOrDefaultAsync();
        }

        public async Task<TEntity> GetFirstAsync<TKey>(Expression<Func<TEntity, TKey>> keySelector, OrderByType orderByType)
        {
            var queryable = GetAll();
            queryable = (orderByType == OrderByType.Ascending) ? queryable.OrderBy(keySelector) : queryable.OrderByDescending(keySelector);
            return await queryable.FirstOrDefaultAsync();
        }


        public async Task<List<TEntity>> FindAllIncludingAsync<TKey>(Expression<Func<TEntity, bool>> match, int page, int pageSize, Expression<Func<TEntity, TKey>> keySelector,
                                                OrderByType orderByType, params Expression<Func<TEntity, object>>[] includeProperties)
        {
            var queryable = FindBy(match);
            queryable = (orderByType == OrderByType.Ascending) ? queryable.OrderBy(keySelector) : queryable.OrderByDescending(keySelector);
            foreach (Expression<Func<TEntity, object>> includeProperty in includeProperties)
            {
                queryable = queryable.Include<TEntity, object>(includeProperty);
            }
            if (page > 0)
            {
                if (page == 0) page = 1;
                int skip = (page - 1) * pageSize;
                queryable = queryable.Skip(skip).Take(pageSize);
            }
            return await queryable.ToListAsync();
        }

      

        public bool Contains(Expression<Func<TEntity, bool>> predicate)
        {
            return GetAll().Count<TEntity>(predicate) > 0;
        }

        public void Delete(Expression<Func<TEntity, bool>> predicate)
        {
            var objects = FindBy(predicate);
            foreach (var obj in objects)
                _dbContext.Set<TEntity>().Remove(obj);
        }

        public void ExecuteProcedure(string procedureCommand, params SqlParameter[] sqlParams)
        {
            ((EntitiesContext)_dbContext).Database.ExecuteSqlCommand(procedureCommand, sqlParams);
        }


        public async Task<TEntity> GetSingleIncludingAsync(TId id, params Expression<Func<TEntity, object>>[] includeProperties)
        {
            IQueryable<TEntity> entities = GetAllIncluding(includeProperties);
            var entity = await Filter<TId>(entities, x => x.Id, id).FirstOrDefaultAsync();
            return entity;
        }

        public async Task<TEntity> FindAsync(Expression<Func<TEntity, bool>> match)
        {
            return await GetAll().SingleOrDefaultAsync(match);
        }

        public async Task<List<TEntity>> FindAllAsync<TKey>(Expression<Func<TEntity, bool>> match, Expression<Func<TEntity, TKey>> keySelector, OrderByType orderByType, int? take, int? skip)
        {
            var queryable = FindAllQueryable(match, keySelector, orderByType, take, skip);
            return await queryable.ToListAsync();
        }

        public List<TEntity> FindAll<TKey>(Expression<Func<TEntity, bool>> match, Expression<Func<TEntity, TKey>> keySelector, OrderByType orderByType, int? take, int? skip)
        {
            var queryable = FindAllQueryable(match, keySelector, orderByType, take, skip);
            return  queryable.ToList();
        }

        private IQueryable<TEntity> FindAllQueryable<TKey>(Expression<Func<TEntity, bool>> match, Expression<Func<TEntity, TKey>> keySelector, OrderByType orderByType, int? take,
                                                  int? skip)
        {
            var queryable = FindBy(match);
            queryable = (orderByType == OrderByType.Ascending)
                            ? queryable.OrderBy(keySelector)
                            : queryable.OrderByDescending(keySelector);
            if (skip.HasValue && skip.Value > 0)
            {
                queryable = queryable.Skip(skip.Value);
            }
            if (take.HasValue && take.Value > 0)
            {
                queryable = queryable.Take(take.Value);
            }
            return queryable;
        }


        public async Task<List<TEntity>> FindAllAsync<TKey>(Expression<Func<TEntity, bool>> match, Expression<Func<TEntity, TKey>> keySelector, OrderByType orderByType, int page, int pageSize)
        {
            var queryable = FindBy(match);
            queryable = (orderByType == OrderByType.Ascending) ? queryable.OrderBy(keySelector) : queryable.OrderByDescending(keySelector);
            if (page > 0)
            {
                if (page == 0) page = 1;
                int skip = (page - 1) * pageSize;
                queryable = queryable.Skip(skip).Take(pageSize);
            }
            return await queryable.ToListAsync();
        }


        public async Task<List<TEntity>> FindAllAsync(Expression<Func<TEntity, bool>> match, int? take, int? skip)
        {
            var queryable = FindBy(match);
            if (skip.HasValue && skip.Value > 0)
            {
                queryable = queryable.Skip(skip.Value);
            }
            if (take.HasValue && take.Value > 0)
            {
                queryable = queryable.Take(take.Value);
            }
            return await queryable.ToListAsync();
        }


        public async Task<TEntity> AddAsync(TEntity t)
        {
            _dbContext.Set<TEntity>().Add(t);
            await _dbContext.SaveChangesAsync();
            return t;
        }

        public async Task<TEntity> EditAsync(TEntity updated, TId id)
        {
            if (updated == null)
                return null;

            TEntity existing = await GetAll().FirstOrDefaultAsync(x => Equals(x.Id, id));
            if (existing != null)
            {
                _dbContext.Entry(existing).CurrentValues.SetValues(updated);
                await _dbContext.SaveChangesAsync();
            }
            return existing;
        }

        public async Task<List<TEntity>> GetAllIncludingAsync<TKey>(int? take, int? skip, Expression<Func<TEntity, TKey>> keySelector, OrderByType orderByType,
                                               params Expression<Func<TEntity, object>>[] includeProperties)
        {
            var queryable = GetAll();
            queryable = (orderByType == OrderByType.Ascending) ? queryable.OrderBy(keySelector) : queryable.OrderByDescending(keySelector);
            foreach (Expression<Func<TEntity, object>> includeProperty in includeProperties)
            {
                queryable = queryable.Include<TEntity, object>(includeProperty);
            }
            if (skip.HasValue && skip.Value > 0)
            {
                queryable = queryable.Skip(skip.Value);
            }
            if (take.HasValue && take.Value > 0)
            {
                queryable = queryable.Take(take.Value);
            }
            return await queryable.ToListAsync();
        }

        public int Count()
        {
            return GetAll().Count();
        }

        public async Task<int> CountAsync()
        {
            return await GetAll().CountAsync();
        }

        public int Count(Expression<Func<TEntity, bool>> match)
        {
            return FindBy(match).Count();
        }

        public async Task<int> CountAsync(Expression<Func<TEntity, bool>> match)
        {
            return await FindBy(match).CountAsync();
        }

        public List<TEntity> FindAllIncluding<TKey>(Expression<Func<TEntity, bool>> match, int? take, int? skip, Expression<Func<TEntity, TKey>> keySelector, OrderByType orderByType,
                                         params Expression<Func<TEntity, object>>[] includeProperties)
        {
            var queryable = FindAllIncludingQueryable(match, take, skip, keySelector, orderByType, includeProperties);
            return queryable.ToList();
        }

        public async Task<List<TEntity>> FindAllIncludingAsync<TKey>(Expression<Func<TEntity, bool>> match, int? take, int? skip, Expression<Func<TEntity, TKey>> keySelector, OrderByType orderByType,
                                                params Expression<Func<TEntity, object>>[] includeProperties)
        {
            var queryable = FindAllIncludingQueryable(match, take, skip, keySelector, orderByType, includeProperties);
            return await queryable.ToListAsync();
        }

        private IQueryable<TEntity> FindAllIncludingQueryable<TKey>(Expression<Func<TEntity, bool>> match, int? take, int? skip, Expression<Func<TEntity, TKey>> keySelector,
                                                           OrderByType orderByType, Expression<Func<TEntity, object>>[] includeProperties)
        {
            var queryable = _dbContext.Set<TEntity>().Where(match);
            queryable = (orderByType == OrderByType.Ascending)
                            ? queryable.OrderBy(keySelector)
                            : queryable.OrderByDescending(keySelector);

            foreach (Expression<Func<TEntity, object>> includeProperty in includeProperties)
            {
                queryable = queryable.Include<TEntity, object>(includeProperty);
            }
            if (skip.HasValue && skip.Value > 0)
            {
                queryable = queryable.Skip(skip.Value);
            }
            if (take.HasValue && take.Value > 0)
            {
                queryable = queryable.Take(take.Value);
            }
            return queryable;
        }

        // Privates

        private PaginatedList<TEntity> Paginate<TKey>(int pageIndex, int pageSize, Expression<Func<TEntity, TKey>> keySelector, Expression<Func<TEntity, bool>> predicate, OrderByType orderByType, params Expression<Func<TEntity, object>>[] includeProperties)
        {

            IQueryable<TEntity> queryable =
                (orderByType == OrderByType.Ascending)
                    ? GetAllIncluding(includeProperties).OrderBy(keySelector)
                    : GetAllIncluding(includeProperties).OrderByDescending(keySelector);

            queryable = (predicate != null) ? queryable.Where(predicate) : queryable;
            PaginatedList<TEntity> paginatedList = queryable.ToPaginatedList(pageIndex, pageSize);

            return paginatedList;
        }

        private IQueryable<TEntity> Filter<TProperty>(IQueryable<TEntity> dbSet,
            Expression<Func<TEntity, TProperty>> property, TProperty value)
            where TProperty : IComparable
        {

            var memberExpression = property.Body as MemberExpression;
            if (memberExpression == null || !(memberExpression.Member is PropertyInfo))
            {

                throw new ArgumentException("Property expected", "property");
            }

            Expression left = property.Body;
            Expression right = Expression.Constant(value, typeof(TProperty));
            Expression searchExpression = Expression.Equal(left, right);
            Expression<Func<TEntity, bool>> lambda = Expression.Lambda<Func<TEntity, bool>>(
                searchExpression, new ParameterExpression[] { property.Parameters.Single() });

            return dbSet.Where(lambda);
        }




    }
}