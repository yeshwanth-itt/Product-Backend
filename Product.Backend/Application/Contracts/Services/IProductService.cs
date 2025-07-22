using Microsoft.AspNetCore.Mvc;
using Product.Backend.Application.Dto;

namespace Product.Backend.Application.Contracts.Services
{
    public interface IProductService
    {
        Task<ProductDto> CreateProductsAsync(CreateProductDto createProductDto);
        Task<IEnumerable<ProductDto>> GetAllProductsAsync();
        Task<ProductDto?> GetProductById(int id);
        Task<bool> UpdateProductAsync(ProductDto productDto);
        Task<bool> DeleteProductAsync(int id);
        Task<bool> IncreaseStockAsync(int id, int quantity);
        Task<bool> DecreaseStockAsync(int id, int quantity);
        Task<List<ProductDto>> GetPagedProductsAsync(int pageNumber, int pageSize);
    }
}
