using Microsoft.AspNetCore.Mvc;
using Moq;
using Product.Backend.API.Controllers;
using Product.Backend.Application.Contracts.Services;
using Product.Backend.Application.Dto;

namespace Product.Backend.UnitTests.API
{
    internal class ProductControllerUnitTests
    {
        private Mock<IProductService> _productServiceMock;
        private ProductController _productController;

        [SetUp]
        public void Setup()
        {
            _productServiceMock = new Mock<IProductService>();
            _productController = new ProductController(_productServiceMock.Object);
        }

        [Test]
        public async Task CreateProduct_Returns_CreatedAtAction_With_Product()
        {
            var inputDto = new ProductDto
            {
                Name = "Test Product",
                Category = "Test Category",
                Description = "Test Desc",
                Stock = 20
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

            var createdAtActionResult = result as CreatedAtActionResult;
            Assert.IsNotNull(createdAtActionResult);
            Assert.AreEqual(nameof(_productController.GetProductById), createdAtActionResult.ActionName);
            Assert.AreEqual(returnedDto.Id, ((ProductDto)createdAtActionResult.Value).Id);
        }
    }
}
