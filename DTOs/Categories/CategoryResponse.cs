namespace OrderFlow.Api.DTOs.Categories;

public class CategoryResponse
{
    public int Id { get; set; }

    public string Name { get; set; } = "";

    public DateTime CreatedAt { get; set; }
}