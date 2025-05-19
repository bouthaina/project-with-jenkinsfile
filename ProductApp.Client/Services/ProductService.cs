using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using ProductApp.Client.Models;

namespace ProductApp.Client.Services
{
    public class ProductService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl = "api/products";

        public ProductService(HttpClient httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        public async Task<List<Product>> GetProductsAsync()
        {
            try
            {
                var products = await _httpClient.GetFromJsonAsync<List<Product>>(_baseUrl);
                return products ?? new List<Product>();
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Erreur lors de la récupération des produits: {ex.Message}");
                return new List<Product>();
            }
        }

        public async Task<Product?> GetProductAsync(int id)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<Product>($"{_baseUrl}/{id}");
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Erreur lors de la récupération du produit {id}: {ex.Message}");
                return null;
            }
        }

        public async Task<Product?> CreateProductAsync(Product product)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));

            try
            {
                var response = await _httpClient.PostAsJsonAsync(_baseUrl, product);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<Product>();
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Erreur lors de la création du produit: {ex.Message}");
                return null;
            }
        }

        public async Task<bool> UpdateProductAsync(int id, Product product)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));

            try
            {
                var response = await _httpClient.PutAsJsonAsync($"{_baseUrl}/{id}", product);
                response.EnsureSuccessStatusCode();
                return true;
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Erreur lors de la mise à jour du produit {id}: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> DeleteProductAsync(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"{_baseUrl}/{id}");
                response.EnsureSuccessStatusCode();
                return true;
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Erreur lors de la suppression du produit {id}: {ex.Message}");
                return false;
            }
        }
    }
}