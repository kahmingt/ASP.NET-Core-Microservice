using Microsoft.EntityFrameworkCore;
using Service.Product.Shared.Database;
using Service.Product.Shared.Database.Entity;
using Service.Product.Work.Repository;
using Service.Product.Work.Utility;

namespace Service.Product.Shared.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _db;
        private IProductRepository _productRepository;
        private OperationHelper<dbProduct> _operationHelper;

        public UnitOfWork(
            ApplicationDbContext db,
            OperationHelper<dbProduct> operationHelper)
        {
            _db = db;
            _operationHelper = operationHelper;
        }

        public IProductRepository ProductRepository
        {
            get
            {
                _productRepository ??= new ProductRepository(_db, _operationHelper);
                return _productRepository;
            }
        }

        public async Task CommitChangesAsync()
        {
            var strategy = _db.Database.CreateExecutionStrategy();
            await strategy.ExecuteAsync(async () =>
            {
                var transaction = await _db.Database.BeginTransactionAsync();
                try
                {
                    await _db.SaveChangesAsync();
                    await transaction.CommitAsync();
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    transaction.Dispose();
                    throw new Exception(ex.Message);
                }
            });
        }
    }

}