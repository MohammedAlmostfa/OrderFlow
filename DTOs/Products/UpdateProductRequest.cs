using System.ComponentModel.DataAnnotations;

namespace OrderFlow.Api.DTOs.Products;

public class UpdateProductRequest
{
    [Required]
    [MaxLength(150)]
    public string Name { get; set; } = "";

    [MaxLength(1000)]
    public string Description { get; set; } = "";

    [Range(typeof(decimal), "0.01", "9999999999999999")]
    public decimal Price { get; set; }

    [Range(0, int.MaxValue)]
    public int StockQuantity { get; set; }

    [Range(1, int.MaxValue)]
    public int CategoryId { get; set; }
}