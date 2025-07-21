
namespace Product.Backend.Application.Contracts.Persistance
{
    public interface IProductRepository
    {
        Task<Domain.Product> CreateAsync(Domain.Product product);
        Task<IEnumerable<Domain.Product>> GetAllAsync();
        Task<Domain.Product?> GetByIdAsync(int id);
        Task UpdateAsync(Domain.Product product);
        Task DeleteAsync(Domain.Product product);
    }

}
