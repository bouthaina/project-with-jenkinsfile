using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using ProductApp.API.Controllers;
using ProductApp.API.Data;
using ProductApp.API.Models;
using Xunit;

namespace ProductApp.API.Tests
{
    public class ProductsControllerTests
    {
        private readonly DbContextOptions<ApplicationDbContext> _options;
        
        public ProductsControllerTests()
        {
            _options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            
            // Initialiser la base de données de test
            using (var context = new ApplicationDbContext(_options))
            {
                context.Products.Add(new Product { Id = 1, Name = "Test Product",Description="Produit pour le test",Price = 19.99m, StockQuantity = 100, Category = "Test" });
                context.SaveChanges();
            }
        }
        
        [Fact]
        public async Task GetProducts_ReturnsAllProducts()
        {
            // Arrange
            using (var context = new ApplicationDbContext(_options))
            {
                var controller = new ProductsController(context);
                
                // Act
                var result = await controller.GetProducts();
                
                // Assert
                var actionResult = Assert.IsType<ActionResult<IEnumerable<Product>>>(result);
                var products = Assert.IsAssignableFrom<IEnumerable<Product>>(actionResult.Value);
                Assert.NotEmpty(products);
            }
        }
        
        [Fact]
        public async Task GetProduct_WithValidId_ReturnsProduct()
        {
            // Arrange
            using (var context = new ApplicationDbContext(_options))
            {
                var controller = new ProductsController(context);
                
                // Act
                var result = await controller.GetProduct(1);
                
                // Assert
                var actionResult = Assert.IsType<ActionResult<Product>>(result);
                var product = Assert.IsType<Product>(actionResult.Value);
                Assert.Equal(1, product.Id);
                Assert.Equal("Test Product", product.Name);
            }
        }
        
        [Fact]
        public async Task CreateProduct_AddsProductToDatabase()
        {
            // Arrange
            var newProduct = new Product { Name = "New Product",Description="Produit pour le test",Price = 29.99m, StockQuantity = 50, Category = "New" };
            
            using (var context = new ApplicationDbContext(_options))
            {
                var controller = new ProductsController(context);
                
                // Act
                var result = await controller.PostProduct(newProduct);
                
                // Assert
                var actionResult = Assert.IsType<ActionResult<Product>>(result);
                var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(actionResult.Result);
                var createdProduct = Assert.IsType<Product>(createdAtActionResult.Value);

                
                Assert.Equal("New Product", createdProduct.Name);
                
                // Vérifier que le produit a bien été ajouté à la base
                var productInDb = await context.Products.FindAsync(createdProduct.Id);
                Assert.NotNull(productInDb);
                Assert.Equal("New Product", productInDb.Name);
            }
        }
    }
}
