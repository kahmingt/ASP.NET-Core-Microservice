using Service.Product.Shared.Database.Entity;
using Service.Product.Work.Repository;

namespace Service.Product.Shared.Repository
{
    public interface IUnitOfWork
    {
        /// <summary>
        /// Northwind <see cref="dbProduct"/> database handler.
        /// </summary>
        IProductRepository ProductRepository { get; }

        Task CommitChangesAsync();

    }
}