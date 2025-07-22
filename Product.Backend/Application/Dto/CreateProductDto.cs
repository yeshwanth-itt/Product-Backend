using System.ComponentModel.DataAnnotations;

namespace Product.Backend.Application.Dto
{
    public class CreateProductDto
    {
        [Range(0, int.MaxValue, ErrorMessage = "Stock value is invalid.")]
        public int Stock { get; set; }

        [Range(0.01, (double)decimal.MaxValue, ErrorMessage = "Price must be greater than zero.")]
        public decimal Price { get; set; }

        [StringLength(30, ErrorMessage = "Name can't exceed 30 characters.")]
        public string Name { get; set; }

        [StringLength(100, ErrorMessage = "Description can't exceed 100 characters.")]
        public string Description { get; set; }

        [StringLength(30, ErrorMessage = "Category can't exceed 30 characters.")]
        public string Category { get; set; }
    }
}
