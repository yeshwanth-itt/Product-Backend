using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Product.Backend.API.Controllers;
using Product.Backend.Application.Contracts.Services;
using Product.Backend.Application.Dto;

namespace Product.Backend.UnitTests.API;

internal class ProductControllerUnitTests
{
    private ILogger<ProductController> _logger;
    private ProductController _productController;
    private Mock<IProductService> _productServiceMock;

    [SetUp]
    public void Setup()
    {
        _productServiceMock = new Mock<IProductService>();
        _logger = new Mock<ILogger<ProductController>>().Object;
        _productController = new ProductController(_productServiceMock.Object, _logger);
    }

    [Test]
    public async Task CreateProduct_ForValidRequest_CreatedAtActionWithProduct()
    {
        var inputDto = new CreateProductDto
        {
            Stock = 20,
            Price = 100,
            Name = "Test Product",
            Category = "Test Category",
            Description = "Test Desc"
        };

        var returnedDto = new ProductDto
        {
            Id = 100001,
            Name = inputDto.Name,
            Category = inputDto.Category,
            Description = inputDto.Description,
            Stock = inputDto.Stock
        };

        _productServiceMock
            .Setup(service => service.CreateProductsAsync(inputDto))
            .ReturnsAsync(returnedDto);

        var result = await _productController.CreateProduct(inputDto);

        var createdAtActionResult = result.Result as CreatedAtActionResult;
        Assert.Multiple(() =>
        {
            Assert.IsNotNull(createdAtActionResult);
            Assert.AreEqual(nameof(_productController.GetProductById), createdAtActionResult.ActionName);
            Assert.AreEqual(returnedDto.Id, ((ProductDto)createdAtActionResult.Value).Id);
        });
    }

    [Test]
    public async Task GetAllProducts_ReturnsProducts()
    {
        var product1 = new ProductDto { Id = 100001, Stock = 10 };
        var product2 = new ProductDto { Id = 100002, Stock = 20 };
        var products = new List<ProductDto> { product1, product2 };
        _productServiceMock.Setup(service => service.GetAllProductsAsync()).ReturnsAsync(products);

        var result = await _productController.GetAllProducts();

        var okResult = result.Result as OkObjectResult;
        Assert.Multiple(() =>
        {
            Assert.IsNotNull(okResult);
            Assert.AreEqual(okResult.StatusCode, 200);
            Assert.AreEqual(((List<ProductDto>)okResult.Value).Count, 2);
        });
    }

    [Test]
    public async Task GetProductById_ProductExists_ReturnsOkWithProduct()
    {
        var productId = 100001;
        var expectedProduct = new ProductDto
        {
            Id = productId,
            Name = "Test Product",
            Price = 100,
            Stock = 10
        };

        _productServiceMock.Setup(s => s.GetProductById(productId))
            .ReturnsAsync(expectedProduct);

        var result = await _productController.GetProductById(productId);

        var okResult = result.Result as OkObjectResult;
        var returnedProduct = okResult.Value as ProductDto;
        Assert.Multiple(() =>
        {
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            Assert.IsNotNull(returnedProduct);
            Assert.AreEqual(expectedProduct.Id, returnedProduct.Id);
            Assert.AreEqual(expectedProduct.Name, returnedProduct.Name);
        });
    }

    [Test]
    public async Task GetProductById_ProductDoesNotExist_ReturnsNotFound()
    {
        var productId = 100300;
        _productServiceMock.Setup(s => s.GetProductById(productId))
            .ReturnsAsync((ProductDto?)null);

        var result = await _productController.GetProductById(productId);

        var notFoundResult = result.Result as NotFoundResult;
        Assert.Multiple(() =>
        {
            Assert.IsNotNull(notFoundResult);
            Assert.AreEqual(404, notFoundResult.StatusCode);
        });
    }

    [Test]
    public async Task UpdateProduct_IdMismatch_ReturnsBadRequest()
    {
        var dto = new ProductDto { Id = 100001, Name = "Product A" };
        var routeId = 100002;

        var result = await _productController.UpdateProduct(routeId, dto);

        var badRequestResult = result as BadRequestObjectResult;
        Assert.IsNotNull(badRequestResult);
        Assert.AreEqual(400, badRequestResult.StatusCode);
        Assert.AreEqual("ID mismatch between route and body.", badRequestResult.Value);
    }

    [Test]
    public async Task UpdateProduct_ProductNotFound_ReturnsNotFound()
    {
        var dto = new ProductDto { Id = 1, Name = "Product A" };
        _productServiceMock.Setup(s => s.UpdateProductAsync(dto)).ReturnsAsync(false);

        var result = await _productController.UpdateProduct(1, dto);

        var notFoundResult = result as NotFoundResult;
        Assert.IsNotNull(notFoundResult);
        Assert.AreEqual(404, notFoundResult.StatusCode);
    }

    [Test]
    public async Task UpdateProduct_SuccessfulUpdate_ReturnsNoContent()
    {
        var dto = new ProductDto { Id = 1, Name = "Product A" };
        _productServiceMock.Setup(s => s.UpdateProductAsync(dto)).ReturnsAsync(true);

        var result = await _productController.UpdateProduct(1, dto);

        var noContentResult = result as OkObjectResult;
        Assert.IsNotNull(noContentResult);
        Assert.AreEqual(200, noContentResult.StatusCode);
    }

    [Test]
    public async Task DeleteProduct_ProductExists_ReturnsNoContent()
    {
        _productServiceMock.Setup(s => s.DeleteProductAsync(1)).ReturnsAsync(true);

        var result = await _productController.DeleteProduct(1);

        Assert.IsInstanceOf<OkObjectResult>(result);
    }

    [Test]
    public async Task DeleteProduct_ProductNotFound_ReturnsNotFound()
    {
        _productServiceMock.Setup(s => s.DeleteProductAsync(1)).ReturnsAsync(false);

        var result = await _productController.DeleteProduct(1);

        Assert.IsInstanceOf<NotFoundResult>(result);
    }


    [Test]
    public async Task IncreaseProductStock_Success_ReturnsOk()
    {
        _productServiceMock.Setup(s => s.IncreaseStockAsync(1, 5)).ReturnsAsync(true);

        var result = await _productController.IncreaseProductStock(1, 5);

        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.AreEqual(200, okResult.StatusCode);
        StringAssert.Contains("Stock increased by", okResult.Value.ToString());
    }

    [Test]
    public async Task IncreaseProductStock_ProductNotFound_ReturnsNotFound()
    {
        _productServiceMock.Setup(s => s.IncreaseStockAsync(1, 5)).ReturnsAsync(false);

        var result = await _productController.IncreaseProductStock(1, 5);

        Assert.IsInstanceOf<NotFoundResult>(result);
    }

    [Test]
    public async Task DecreaseProductStock_Success_ReturnsOk()
    {
        _productServiceMock.Setup(s => s.DecreaseStockAsync(1, 3)).ReturnsAsync(true);

        var result = await _productController.DecreaseProductStock(1, 3);

        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.AreEqual(200, okResult.StatusCode);
        StringAssert.Contains("Stock decreased by", okResult.Value.ToString());
    }

    [Test]
    public async Task DecreaseProductStock_ProductNotFoundOrInsufficient_ReturnsNotFound()
    {
        _productServiceMock.Setup(s => s.DecreaseStockAsync(1, 3)).ReturnsAsync(false);

        var result = await _productController.DecreaseProductStock(1, 3);

        Assert.IsInstanceOf<NotFoundResult>(result);
    }
}