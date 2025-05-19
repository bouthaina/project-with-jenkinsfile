using System;
using System.ComponentModel.DataAnnotations;

namespace ProductApp.Client.Models
{
    public class Product
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "Le nom est obligatoire")]
        [StringLength(100, ErrorMessage = "Le nom ne peut pas dépasser 100 caractères")]
        public string Name { get; set; }
        
        [StringLength(500, ErrorMessage = "La description ne peut pas dépasser 500 caractères")]
        public string Description { get; set; }
        
        [Required(ErrorMessage = "Le prix est obligatoire")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Le prix doit être supérieur à 0")]
        public decimal Price { get; set; }
        
        [Range(0, int.MaxValue, ErrorMessage = "La quantité en stock ne peut pas être négative")]
        public int StockQuantity { get; set; }
        
        public string Category { get; set; }
        
       
        
       
    }
}
