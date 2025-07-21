using AutoMapper;
using Product.Backend.Application.Contracts.Persistance;
using Product.Backend.Application.Contracts.Services;
using Product.Backend.Application.Dto;

namespace Product.Backend.Application.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<ProductService> _logger;

        public ProductService(IProductRepository productRepository, IMapper mapper, ILogger<ProductService> logger)
        {
            _productRepository = productRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ProductDto> CreateProductsAsync(ProductDto productDto)
        {
            var product = _mapper.Map<Domain.Product>(productDto);
            var createdProduct = await _productRepository.CreateAsync(product);

            return _mapper.Map<ProductDto>(createdProduct);
        }

        public async Task<IEnumerable<ProductDto>> GetAllProductsAsync()
        {
            var products = await _productRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<ProductDto>>(products);
        }

        public async Task<ProductDto?> GetProductById(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
            {
                _logger.LogWarning("Product with ID {ProductId} not found.", id);
            }

            return _mapper.Map<ProductDto>(product);
        }

        public async Task<bool> UpdateProductAsync(ProductDto productDto)
        {
            var existingProduct = await _productRepository.GetByIdAsync(productDto.Id);
            if (existingProduct is null)
            {
                _logger.LogWarning("Cannot update. Product with ID {ProductId} not found.", productDto.Id);
                return false;
            }

            _mapper.Map(productDto, existingProduct);
            await _productRepository.UpdateAsync(existingProduct);
            return true;
        }

        public async Task<bool> DeleteProductAsync(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product is null)
            {
                _logger.LogWarning("Cannot delete. Product with ID {ProductId} not found.", id);
                return false;
            }

            await _productRepository.DeleteAsync(product);
            return true;
        }

        public async Task<bool> IncreaseStockAsync(int id, int quantity)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product is null)
            {
                _logger.LogWarning("Product with ID {ProductId} not found for stock increment.", id);
                return false;
            }

            product.Stock += quantity;
            await _productRepository.UpdateAsync(product);
            return true;
        }

        public async Task<bool> DecreaseStockAsync(int id, int quantity)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product is null)
            {
                _logger.LogWarning("Product with ID {ProductId} not found for stock decrement.", id);
                return false;
            }

            if (product.Stock < quantity)
            {
                _logger.LogWarning("Insufficient stock for product ID {ProductId}. Available: {Stock}, Requested: {Quantity}", id, product.Stock, quantity);
                return false;
            }

            product.Stock -= quantity;
            await _productRepository.UpdateAsync(product);
            return true;
        }

        public async Task<List<ProductDto>> GetPagedProductsAsync(int pageNumber, int pageSize)
        {
            var products = await _productRepository.GetPagedAsync(pageNumber, pageSize);
            return _mapper.Map<List<ProductDto>>(products);
        }
    }
}
