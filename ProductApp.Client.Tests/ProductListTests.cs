using System.Collections.Generic;
using Bunit;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using ProductApp.Client.Models;
using ProductApp.Client.Pages;
using ProductApp.Client.Pages.Products;
using ProductApp.Client.Services;
using Xunit;

namespace ProductApp.Client.Tests
{
    public class ProductListTests : TestContext
    {
        [Fact]
    public async Task GetProductsAsync_ReturnsList()
    {
        // Arrange
        var httpClient = new HttpClient
        {
            BaseAddress = new Uri("https://jsonplaceholder.typicode.com/") // ou l’URL de test de ton API
        };
        var service = new ProductService(httpClient);

        // Act
        var products = await service.GetProductsAsync();

        // Assert
        Assert.NotNull(products);
        Assert.IsType<System.Collections.Generic.List<Product>>(products);
    }

}
}
