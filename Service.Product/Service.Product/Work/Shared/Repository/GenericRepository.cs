using Microsoft.EntityFrameworkCore;
using Service.Product.Shared.Database;
using System.Linq.Expressions;

namespace Service.Product.Shared.Repository;

public abstract class GenericRepository<T> : IGenericRepository<T> where T : class
{
    protected ApplicationDbContext _db { get; set; }

    public GenericRepository(ApplicationDbContext db)
    {
        _db = db;
    }

    /// <summary>
    /// Return sequence of objects.
    /// </summary>
    public IQueryable<T> GetAll() => _db.Set<T>().AsNoTracking();

    /// <summary>
    /// Return sequence of objects based on a predicate.
    /// </summary>
    public IQueryable<T> GetAll(Expression<Func<T, bool>> expression) => _db.Set<T>().Where(expression).AsNoTracking();

    /// <summary>
    /// Return single object.
    /// </summary>
    public IQueryable<T> GetSingle() => _db.Set<T>().AsNoTracking();

    /// <summary>
    /// Return single object based on a predicate.
    /// </summary>
    public IQueryable<T> GetSingle(Expression<Func<T, bool>> expression) => _db.Set<T>().Where(expression).AsNoTracking();

    /// <summary>
    /// Add new entity using DbSet<T>.Add(T entity)
    /// </summary>
    public void Create(T entity) => _db.Set<T>().Add(entity);

    /// <summary>
    /// Update entity using DbSet<T>.Update(T entity)
    /// </summary>
    public void Update(T entity) => _db.Set<T>().Update(entity);

    /// <summary>
    /// Delete entity using DbSet<T>.Remove(T entity)
    /// </summary>
    public void Delete(T entity) => _db.Set<T>().Remove(entity);

}