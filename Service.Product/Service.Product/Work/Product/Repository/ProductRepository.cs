using Microsoft.EntityFrameworkCore;
using Service.Product.Shared.Database;
using Service.Product.Shared.Database.Entity;
using Service.Product.Shared.Repository;
using Service.Product.Shared.Utility;
using Service.Product.Work.Utility;

namespace Service.Product.Work.Repository
{
    public class ProductRepository : GenericRepository<dbProduct>, IProductRepository
    {
        protected ApplicationDbContext _db { get; set; }
        private OperationHelper<dbProduct> _operationHelper;

        public ProductRepository(
            ApplicationDbContext db,
            OperationHelper<dbProduct> operationHelper)
            : base(db)
        {
            _db = db;
            _operationHelper = operationHelper;
        }

        public async Task CreateProductAsync(dbProduct product)
        {
            Create(product);
        }

        public async Task DeleteProductByIdAsync(dbProduct product)
        {
            product.IsDeleted = true;
            Update(product);
        }

        public async Task<dbProduct> GetProductDetailsByIdAsync(int id)
        {
            var model = await GetSingle(x => x.ProductId == id && !x.IsDeleted)
                                .Include(x => x.Category)
                                .FirstOrDefaultAsync();
            return model!;
        }

        public async Task<PagedList<dbProduct>> GetProductListAsync(ProductQueryableParameter parameter)
        {
            var model = (IQueryable<dbProduct>)GetAll(x => !x.IsDeleted).Include(x => x.Category);

            // Sorting & Filtering
            model = _operationHelper.RunAll(model, parameter);

            return PagedList<dbProduct>.ToPagedList(
                            model,
                            parameter.PageNumber,
                            parameter.PageSize);
        }

        public async Task UpdateProductDetailsByIdAsync(dbProduct products)
        {
            _db.Entry(products).Property(x => x.ProductId).IsModified = false;
            Update(products);
        }

    }
}
