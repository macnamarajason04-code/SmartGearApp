using System.ComponentModel.DataAnnotations;

namespace SmartGearApp.Models
{
    public class Product
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Product name is required")]
        [StringLength(50, ErrorMessage = "Product name cannot exceed 50 characters")]
        public string? Name { get; set; }

        [Required(ErrorMessage = "Category is required")]
        public string? Category { get; set; }

        [Range(0.01, 100000, ErrorMessage = "Price must be between 0.01 and 100000")]
        public decimal Price { get; set; }

        [Range(0, 1000, ErrorMessage = "Quantity must be between 0 and 1000")]
        public int Quantity { get; set; }

        public decimal CalculateTotalPrice()
        {
            return Price * Quantity;
        }
    }
}
