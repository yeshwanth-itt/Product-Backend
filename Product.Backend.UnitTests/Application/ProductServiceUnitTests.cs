using AutoMapper;
using Microsoft.Extensions.Logging;
using Moq;
using Product.Backend.Application.Contracts.Persistance;
using Product.Backend.Application.Dto;
using Product.Backend.Application.Services;

namespace Product.Backend.UnitTests.Application;

internal class ProductServiceUnitTests
{
    private ILogger<ProductService> _logger;
    private IMapper _mapper;
    private Mock<IProductRepository> _productRepositoryMock;
    private ProductService _productService;

    [SetUp]
    public void Setup()
    {
        _productRepositoryMock = new Mock<IProductRepository>();
        _logger = new Mock<ILogger<ProductService>>().Object;

        var config = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<ProductDto, Domain.Product>();
            cfg.CreateMap<Domain.Product, ProductDto>();
        });
        _mapper = config.CreateMapper();

        _productService = new ProductService(_productRepositoryMock.Object, _mapper, _logger);
    }

    [Test]
    public async Task UpdateProductAsync_ReturnsTrue_WhenProductExists()
    {
        var productDto = new ProductDto { Id = 100001, Name = "Updated", Price = 100, Stock = 50 };
        var existingProduct = new Domain.Product { Id = 100001, Name = "Old", Price = 90, Stock = 40 };

        _productRepositoryMock.Setup(repo => repo.GetByIdAsync(productDto.Id))
            .ReturnsAsync(existingProduct);

        _productRepositoryMock.Setup(repo => repo.UpdateAsync(It.IsAny<Domain.Product>()))
            .Returns(Task.CompletedTask);

        var result = await _productService.UpdateProductAsync(productDto);

        Assert.IsTrue(result);
        _productRepositoryMock.Verify(repo => repo.UpdateAsync(It.Is<Domain.Product>(
            p => p.Name == "Updated" && p.Price == 100 && p.Stock == 50)), Times.Once);
    }

    [Test]
    public async Task UpdateProductAsync_ReturnsFalse_WhenProductDoesNotExist()
    {
        var productDto = new ProductDto { Id = 1, Name = "Updated", Price = 100, Stock = 50 };

        _productRepositoryMock.Setup(repo => repo.GetByIdAsync(productDto.Id))
            .ReturnsAsync((Domain.Product)null!);

        var result = await _productService.UpdateProductAsync(productDto);

        Assert.IsFalse(result);
        _productRepositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<Domain.Product>()), Times.Never);
    }
}