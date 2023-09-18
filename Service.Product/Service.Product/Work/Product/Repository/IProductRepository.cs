using Service.Product.Shared.Database.Entity;
using Service.Product.Shared.Repository;
using Service.Product.Shared.Utility;
using Service.Product.Work.Utility;

namespace Service.Product.Work.Repository
{
    public interface IProductRepository : IGenericRepository<dbProduct>
    {
        /// <summary>
        /// Create new product details asynchronously.
        /// </summary>
        Task CreateProductAsync(dbProduct product);

        /// <summary>
        /// Delete a specified Product, if exist.
        /// </summary>
        Task DeleteProductByIdAsync(dbProduct product);

        /// <summary>
        /// Finds and returns the product details, if any, with the specified product id.
        /// </summary>
        Task<dbProduct> GetProductDetailsByIdAsync(int id);

        /// <summary>
        /// Get entire product list.
        /// </summary>
        Task<PagedList<dbProduct>> GetProductListAsync(ProductQueryableParameter parameter);

        /// <summary>
        /// Update product details by id.
        /// </summary>
        Task UpdateProductDetailsByIdAsync(dbProduct product);

    }
}
