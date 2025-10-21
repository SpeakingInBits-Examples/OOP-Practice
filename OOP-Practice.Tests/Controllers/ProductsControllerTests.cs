using Microsoft.AspNetCore.Mvc;
using Moq;
using OOP_Practice.Controllers;
using OOP_Practice.Models;
using OOP_Practice.Services;

namespace OOP_Practice.Tests.Controllers;

public class ProductsControllerTests
{
    private readonly Mock<IProductService> _mockProductService;
    private readonly ProductsController _controller;

    public ProductsControllerTests()
    {
        _mockProductService = new Mock<IProductService>();
        _controller = new ProductsController(_mockProductService.Object);
    }

    #region Index Tests

    [Fact]
    public async Task Index_ReturnsViewResult_WithListOfProducts()
    {
        // Arrange
        List<Product> products = new()
        {
            new Product { Id = 1, Name = "Product 1", Price = 10.99m },
            new Product { Id = 2, Name = "Product 2", Price = 20.99m }
        };
        _mockProductService.Setup(s => s.GetAllProductsAsync())
            .ReturnsAsync(products);

        // Act
        var result = await _controller.Index();

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsAssignableFrom<IEnumerable<Product>>(viewResult.Model);
        Assert.Equal(2, model.Count());
    }

    [Fact]
    public async Task Index_ReturnsViewResult_WithEmptyList_WhenNoProducts()
    {
        // Arrange
        _mockProductService.Setup(s => s.GetAllProductsAsync())
            .ReturnsAsync(new List<Product>());

        // Act
        var result = await _controller.Index();

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsAssignableFrom<IEnumerable<Product>>(viewResult.Model);
        Assert.Empty(model);
    }

    #endregion

    #region Details Tests

    [Fact]
    public async Task Details_ReturnsNotFound_WhenIdIsNull()
    {
        // Act
        var result = await _controller.Details(null);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Details_ReturnsNotFound_WhenProductDoesNotExist()
    {
        // Arrange
        _mockProductService.Setup(s => s.GetProductByIdAsync(It.IsAny<int>()))
            .ReturnsAsync((Product?)null);

        // Act
        var result = await _controller.Details(1);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Details_ReturnsViewResult_WithProduct_WhenProductExists()
    {
        // Arrange
        var product = new Product { Id = 1, Name = "Test Product", Price = 15.99m };
        _mockProductService.Setup(s => s.GetProductByIdAsync(1))
            .ReturnsAsync(product);

        // Act
        var result = await _controller.Details(1);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<Product>(viewResult.Model);
        Assert.Equal(1, model.Id);
        Assert.Equal("Test Product", model.Name);
        Assert.Equal(15.99m, model.Price);
    }

    #endregion

    #region Create Tests

    [Fact]
    public void Create_Get_ReturnsViewResult()
    {
        // Act
        var result = _controller.Create();

        // Assert
        Assert.IsType<ViewResult>(result);
    }

    [Fact]
    public async Task Create_Post_ReturnsViewResult_WhenModelStateIsInvalid()
    {
        // Arrange
        var product = new Product { Id = 1, Name = "Test", Price = 10.99m };
        _controller.ModelState.AddModelError("Name", "Required");

        // Act
        var result = await _controller.Create(product);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal(product, viewResult.Model);
    }

    [Fact]
    public async Task Create_Post_RedirectsToIndex_WhenModelStateIsValid()
    {
        // Arrange
        var product = new Product { Id = 0, Name = "New Product", Price = 25.99m };
        _mockProductService.Setup(s => s.AddProductAsync(It.IsAny<Product>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.Create(product);

        // Assert
        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirectResult.ActionName);
        _mockProductService.Verify(s => s.AddProductAsync(product), Times.Once);
    }

    #endregion

    #region Edit Tests

    [Fact]
    public async Task Edit_Get_ReturnsNotFound_WhenIdIsNull()
    {
        // Act
        var result = await _controller.Edit(null);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Edit_Get_ReturnsNotFound_WhenProductDoesNotExist()
    {
        // Arrange
        _mockProductService.Setup(s => s.GetProductByIdAsync(It.IsAny<int>()))
            .ReturnsAsync((Product?)null);

        // Act
        var result = await _controller.Edit(1);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Edit_Get_ReturnsViewResult_WithProduct_WhenProductExists()
    {
        // Arrange
        var product = new Product { Id = 1, Name = "Test Product", Price = 15.99m };
        _mockProductService.Setup(s => s.GetProductByIdAsync(1))
            .ReturnsAsync(product);

        // Act
        var result = await _controller.Edit(1);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<Product>(viewResult.Model);
        Assert.Equal(1, model.Id);
    }

    [Fact]
    public async Task Edit_Post_ReturnsNotFound_WhenIdDoesNotMatchProductId()
    {
        // Arrange
        var product = new Product { Id = 2, Name = "Test", Price = 10.99m };

        // Act
        var result = await _controller.Edit(1, product);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Edit_Post_ReturnsViewResult_WhenModelStateIsInvalid()
    {
        // Arrange
        var product = new Product { Id = 1, Name = "Test", Price = 10.99m };
        _controller.ModelState.AddModelError("Name", "Required");

        // Act
        var result = await _controller.Edit(1, product);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal(product, viewResult.Model);
    }

    [Fact]
    public async Task Edit_Post_RedirectsToIndex_WhenModelStateIsValid()
    {
        // Arrange
        var product = new Product { Id = 1, Name = "Updated Product", Price = 30.99m };
        _mockProductService.Setup(s => s.UpdateProductAsync(It.IsAny<Product>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.Edit(1, product);

        // Assert
        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirectResult.ActionName);
        _mockProductService.Verify(s => s.UpdateProductAsync(product), Times.Once);
    }

    #endregion

    #region Delete Tests

    [Fact]
    public async Task Delete_Get_ReturnsNotFound_WhenIdIsNull()
    {
        // Act
        var result = await _controller.Delete(null);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Delete_Get_ReturnsNotFound_WhenProductDoesNotExist()
    {
        // Arrange
        _mockProductService.Setup(s => s.GetProductByIdAsync(It.IsAny<int>()))
            .ReturnsAsync((Product?)null);

        // Act
        var result = await _controller.Delete(1);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Delete_Get_ReturnsViewResult_WithProduct_WhenProductExists()
    {
        // Arrange
        var product = new Product { Id = 1, Name = "Test Product", Price = 15.99m };
        _mockProductService.Setup(s => s.GetProductByIdAsync(1))
            .ReturnsAsync(product);

        // Act
        var result = await _controller.Delete(1);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<Product>(viewResult.Model);
        Assert.Equal(1, model.Id);
    }

    [Fact]
    public async Task DeleteConfirmed_RedirectsToIndex_AfterDeletion()
    {
        // Arrange
        _mockProductService.Setup(s => s.DeleteProductAsync(It.IsAny<int>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.DeleteConfirmed(1);

        // Assert
        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirectResult.ActionName);
        _mockProductService.Verify(s => s.DeleteProductAsync(1), Times.Once);
    }

    [Fact]
    public async Task DeleteConfirmed_CallsServiceWithCorrectId()
    {
        // Arrange
        int productId = 42;
        _mockProductService.Setup(s => s.DeleteProductAsync(productId))
            .Returns(Task.CompletedTask);

        // Act
        await _controller.DeleteConfirmed(productId);

        // Assert
        _mockProductService.Verify(s => s.DeleteProductAsync(productId), Times.Once);
    }

    #endregion
}
