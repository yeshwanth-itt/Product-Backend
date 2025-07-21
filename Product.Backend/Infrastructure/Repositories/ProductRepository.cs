using Microsoft.EntityFrameworkCore;
using Product.Backend.Application.Contracts.Persistance;

namespace Product.Backend.Infrastructure.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly ProductDbContext _context;
        private readonly ILogger<ProductRepository> _logger;

        public ProductRepository(ProductDbContext context, ILogger<ProductRepository>logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Domain.Product> CreateAsync(Domain.Product product)
        {
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();

            return product;
        }

        public async Task<IEnumerable<Domain.Product>> GetAllAsync()
        {
            return await _context.Products.AsNoTracking().ToListAsync();
        }

        public async Task<Domain.Product?> GetByIdAsync(int id)
        {
            return await _context.Products.AsNoTracking().SingleOrDefaultAsync(p => p.Id == id);
        }

        public async Task UpdateAsync(Domain.Product product)
        {
            try
            {
                _context.Products.Update(product);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogWarning("Concurrency conflict while updating Product with ID {Id}", product.Id);
                throw new DbUpdateConcurrencyException("The product was modified by another user.", ex);
            }
        }

        public async Task DeleteAsync(Domain.Product product)
        {
            try
            {
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogWarning("Concurrency conflict while deleting Product with ID {Id}", product.Id);
                throw new DbUpdateConcurrencyException("The product was modified or deleted by another user.", ex);
            }
        }

        public async Task<List<Domain.Product>> GetPagedAsync(int pageNumber, int pageSize)
        {
            return await _context.Products
                .AsNoTracking()
                .OrderBy(p => p.Id)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }
    }
}
