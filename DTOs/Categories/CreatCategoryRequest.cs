using System.ComponentModel.DataAnnotations;

namespace OrderFlow.Api.DTOs.Categories;

public class CreateCategoryRequest
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = "";
}