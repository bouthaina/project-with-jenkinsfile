using System;
using System.ComponentModel.DataAnnotations;

namespace ProductApp.API.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        [StringLength(100 )]
        public string Name { get; set; }
        
        [StringLength(500)]
        public string Description { get; set; }
        
        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal Price { get; set; }
        
        public int StockQuantity { get; set; }
        
        public string Category { get; set; }
        
       
        
    }
}
