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

        public ProductService(IProductRepository productRepository, IMapper mapper)
        {
            _productRepository = productRepository;
            _mapper = mapper;
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
            return _mapper.Map<ProductDto>(product);
        }

        public async Task<bool> UpdateProductAsync(ProductDto productDto)
        {
            var existingProduct = await _productRepository.GetByIdAsync(productDto.Id);
            if (existingProduct is null)
            {
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
                return false;
            }

            product.Stock += quantity;
            await _productRepository.UpdateAsync(product);
            return true;
        }

        public async Task<bool> DecreaseStockAsync(int id, int quantity)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product is null || product.Stock < quantity)
            {
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
