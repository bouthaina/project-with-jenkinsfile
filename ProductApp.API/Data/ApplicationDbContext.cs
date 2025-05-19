using Microsoft.EntityFrameworkCore;
using ProductApp.API.Models;

namespace ProductApp.API.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Seed data (optional)
            modelBuilder.Entity<Product>().HasData(
                new Product
                {
                    Id = 1,
                    Name = "Produit exemple",
                    Description = "Ceci est un produit exemple",
                    Price = 19.99m,
                    StockQuantity = 100,
                    Category = "Électronique"
                }
            );
        }
    }
}
