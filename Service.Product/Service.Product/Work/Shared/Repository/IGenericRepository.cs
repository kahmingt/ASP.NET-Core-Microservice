using System.Linq.Expressions;

namespace Service.Product.Shared.Repository;

public interface IGenericRepository<T>
{
    IQueryable<T> GetAll();
    IQueryable<T> GetAll(Expression<Func<T, bool>> expression);
    IQueryable<T> GetSingle();
    void Create(T entity);
    void Update(T entity);
    void Delete(T entity);
}
